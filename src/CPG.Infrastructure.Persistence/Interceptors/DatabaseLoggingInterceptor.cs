using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Interceptors;

public class DatabaseLoggingInterceptor(
    ILogService logService,
    IHttpContextAccessor httpContextAccessor,
    ICurrentUser currentUser) : DbCommandInterceptor
{
    private readonly ILogService _logService = logService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ICurrentUser _currentUser = currentUser;

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogCommand(command, eventData, null);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData, null);
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        LogCommand(command, null, eventData.Exception);
        base.CommandFailed(command, eventData);
    }

    public override async Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        LogCommand(command, null, eventData.Exception);
        await base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void LogCommand(DbCommand command, CommandExecutedEventData? executedEventData, Exception? exception)
    {
        var duration = executedEventData?.Duration.TotalMilliseconds ?? 0;
        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var commandText = command.CommandText;
        var parameters = command.Parameters.Count > 0
            ? string.Join(", ", command.Parameters.Cast<DbParameter>().Select(p => $"{p.ParameterName}={p.Value}"))
            : "No parameters";

        var requestBody = $"Command: {commandText}\nParameters: {parameters}";

        if (exception != null)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: "EntityFramework",
                providerName: "SqlServer",
                requestUri: "DbCommand",
                requestBody: requestBody,
                responseBody: exception.Message,
                exception: exception,
                serviceType: Enums.ServiceType.Database,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                correlationId: httpContext?.TraceIdentifier,
                userId: _currentUser.UserId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogError(callLog);
        }
        else
        {
            var callLog = CallLogModel.CreateSuccess(
                serviceName: "EntityFramework",
                providerName: "SqlServer",
                requestUri: "DbCommand",
                requestBody: requestBody,
                responseBody: $"Executed successfully in {duration:F2}ms",
                serviceType: Enums.ServiceType.Database,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                correlationId: httpContext?.TraceIdentifier,
                userId: _currentUser.UserId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: 200
            );
            _logService.LogWarning(callLog); // Log as warning for slow queries
        }
    }
}
