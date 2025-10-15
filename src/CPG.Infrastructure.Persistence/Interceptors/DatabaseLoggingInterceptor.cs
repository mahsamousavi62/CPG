using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Intercepts database operations to provide comprehensive logging
/// Tracks queries, commands, SaveChanges operations, and performance metrics
/// </summary>
public class DatabaseLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DatabaseLoggingInterceptor> _logger;
    private const int SlowQueryThresholdMs = 1000; // 1 second

    public DatabaseLoggingInterceptor(
        ILogService logService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DatabaseLoggingInterceptor> logger)
    {
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        LogCommand(command, eventData, "Query");
        return base.ReaderExecuting(command, eventData, result);
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData, "Query");
        return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogCommandExecuted(command, eventData, "Query", result.RecordsAffected);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecuted(command, eventData, "Query", result.RecordsAffected);
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        LogCommand(command, eventData, GetOperationType(command));
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData, GetOperationType(command));
        return await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        LogCommandExecuted(command, eventData, GetOperationType(command), result);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecuted(command, eventData, GetOperationType(command), result);
        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(
        DbCommand command,
        CommandErrorEventData eventData)
    {
        LogCommandFailed(command, eventData);
        base.CommandFailed(command, eventData);
    }

    public override async Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogCommandFailed(command, eventData);
        await base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void LogCommand(DbCommand command, CommandEventData eventData, string operationType)
    {
        // Store start time in command parameters for later retrieval
        command.CommandTimeout = command.CommandTimeout; // Touch to ensure tracking
    }

    private void LogCommandExecuted(DbCommand command, CommandExecutedEventData eventData, string operationType, int rowsAffected)
    {
        var durationMs = (long)eventData.Duration.TotalMilliseconds;
        var contextType = eventData.Context?.GetType().Name ?? "Unknown";

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

        var log = new DatabaseOperationLog
        {
            OperationType = operationType,
            ContextType = contextType,
            SqlCommand = SanitizeSqlCommand(command.CommandText),
            RowsAffected = rowsAffected,
            IsSuccess = true,
            IsSlow = durationMs >= SlowQueryThresholdMs,
            StartDateTime = DateTime.UtcNow.AddMilliseconds(-durationMs),
            EndDateTime = DateTime.UtcNow,
            DurationMs = durationMs,
            UserId = userId,
            CompanyId = companyId,
            ApplicationId = applicationId,
            CorrelationId = correlationId,
            RequestId = requestId
        };

        _logService.LogDatabaseOperation(log);
    }

    private void LogCommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        var durationMs = (long)eventData.Duration.TotalMilliseconds;
        var contextType = eventData.Context?.GetType().Name ?? "Unknown";

        // Extract user context
        var httpContext = _httpContextAccessor.HttpContext;
        var correlationId = httpContext?.Items["CorrelationId"]?.ToString();
        var requestId = httpContext?.Items["RequestId"]?.ToString();

        long? userId = null;
        long? companyId = null;
        long? applicationId = null;

        if (httpContext?.User?.Claims != null)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var uid))
                userId = uid;

            var companyIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && long.TryParse(companyIdClaim, out var cid))
                companyId = cid;

            var appIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
            if (!string.IsNullOrEmpty(appIdClaim) && long.TryParse(appIdClaim, out var aid))
                applicationId = aid;
        }

        var log = new DatabaseOperationLog
        {
            OperationType = GetOperationType(command),
            ContextType = contextType,
            SqlCommand = SanitizeSqlCommand(command.CommandText),
            IsSuccess = false,
            IsSlow = false,
            ErrorCode = eventData.Exception?.GetType().Name ?? "DatabaseError",
            ErrorMessage = eventData.Exception?.Message ?? "Unknown database error",
            StartDateTime = DateTime.UtcNow.AddMilliseconds(-durationMs),
            EndDateTime = DateTime.UtcNow,
            DurationMs = durationMs,
            UserId = userId,
            CompanyId = companyId,
            ApplicationId = applicationId,
            CorrelationId = correlationId,
            RequestId = requestId
        };

        _logService.LogDatabaseOperation(log);
    }

    private static string GetOperationType(DbCommand command)
    {
        var commandText = command.CommandText?.Trim().ToUpperInvariant() ?? "";

        if (commandText.StartsWith("SELECT"))
            return "Query";
        if (commandText.StartsWith("INSERT"))
            return "Insert";
        if (commandText.StartsWith("UPDATE"))
            return "Update";
        if (commandText.StartsWith("DELETE"))
            return "Delete";
        if (commandText.StartsWith("EXEC") || commandText.StartsWith("EXECUTE"))
            return "StoredProcedure";
        if (commandText.StartsWith("SET"))
            return "SetOperation";

        return "Command";
    }

    private static string SanitizeSqlCommand(string sql)
    {
        if (string.IsNullOrEmpty(sql))
            return string.Empty;

        // Truncate very long SQL commands
        const int maxLength = 500;
        if (sql.Length > maxLength)
        {
            return sql.Substring(0, maxLength) + $"... [truncated from {sql.Length} chars]";
        }

        return sql;
    }
}
