using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace CPG.Infrastructure.Persistence.Interceptors;

public class DatabaseLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<DatabaseLoggingInterceptor> _logger;
    private readonly int _slowQueryThresholdMs;
    private readonly Dictionary<DbCommand, QueryContext> _queryContexts = new();

    public DatabaseLoggingInterceptor(
        ILogger<DatabaseLoggingInterceptor> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _slowQueryThresholdMs = configuration.GetValue<int>("Infrastructure:Database:Logging:SlowQueryThresholdMs", 5000);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        _queryContexts[command] = new QueryContext
        {
            StartTime = DateTime.UtcNow,
            ContextType = eventData.Context?.GetType().Name  // "ReadDbContext" or "WriteDbContext"
        };

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (_queryContexts.TryGetValue(command, out var context))
        {
            var duration = eventData.Duration.TotalMilliseconds;

            // لاگ همه query ها در سطح Information
            _logger.LogInformation(
                "[Database] {ContextType} Query executed in {DurationMs}ms | Rows: {RowsAffected}",
                context.ContextType, duration, result.RecordsAffected);

            // تشخیص Slow Query
            if (duration > _slowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "[Database] SLOW QUERY detected ({DurationMs}ms) | Context: {ContextType} | Query: {QueryText}",
                    duration, context.ContextType, command.CommandText);
            }

            _queryContexts.Remove(command);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var duration = eventData.Duration.TotalMilliseconds;

        _logger.LogInformation(
            "[Database] NonQuery executed in {DurationMs}ms | Context: {ContextType} | Rows: {RowsAffected}",
            duration, eventData.Context?.GetType().Name, result);

        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    private class QueryContext
    {
        public DateTime StartTime { get; set; }
        public string ContextType { get; set; }
    }
}
