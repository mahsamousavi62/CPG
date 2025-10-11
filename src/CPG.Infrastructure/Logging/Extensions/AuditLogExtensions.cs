using CPG.Domain.AggregateModels.AuditLogAggregate;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;

namespace CPG.Infrastructure.Logging.Extensions;

/// <summary>
/// Extension methods for ILogger to support structured audit logging
/// </summary>
public static class AuditLogExtensions
{
    /// <summary>
    /// Logs an audit entry with structured information
    /// </summary>
    public static void LogAudit(this ILogger logger, AuditLog auditLog)
    {
        using (LogContext.PushProperty("AuditLog", auditLog, destructureObjects: true))
        using (LogContext.PushProperty("LogId", auditLog.LogId.Value))
        using (LogContext.PushProperty("CorrelationId", auditLog.CorrelationId))
        {
            var logLevel = auditLog.Level switch
            {
                AuditLevel.Information => LogLevel.Information,
                AuditLevel.Warning => LogLevel.Warning,
                AuditLevel.Error => LogLevel.Error,
                _ => LogLevel.Information
            };

            logger.Log(logLevel, "[AuditLog] {Summary} | AuditLog: {@AuditLog}",
                auditLog.GetSummary(), auditLog);
        }
    }

    /// <summary>
    /// Logs a successful audit entry
    /// </summary>
    public static void LogAuditSuccess(this ILogger logger, AuditLog auditLog)
    {
        if (auditLog.Level != AuditLevel.Information)
        {
            auditLog = auditLog with { Level = AuditLevel.Information };
        }

        using (LogContext.PushProperty("AuditLog", auditLog, destructureObjects: true))
        using (LogContext.PushProperty("LogId", auditLog.LogId.Value))
        using (LogContext.PushProperty("CorrelationId", auditLog.CorrelationId))
        {
            logger.LogInformation("[AuditLog] SUCCESS | {Summary} | {@AuditLog}",
                auditLog.GetSummary(), auditLog);
        }
    }

    /// <summary>
    /// Logs an error audit entry
    /// </summary>
    public static void LogAuditError(this ILogger logger, AuditLog auditLog, Exception? exception = null)
    {
        if (auditLog.Level != AuditLevel.Error)
        {
            auditLog = auditLog with { Level = AuditLevel.Error };
        }

        using (LogContext.PushProperty("AuditLog", auditLog, destructureObjects: true))
        using (LogContext.PushProperty("LogId", auditLog.LogId.Value))
        using (LogContext.PushProperty("CorrelationId", auditLog.CorrelationId))
        {
            if (exception != null)
            {
                logger.LogError(exception, "[AuditLog] ERROR | {Summary} | {@AuditLog}",
                    auditLog.GetSummary(), auditLog);
            }
            else
            {
                logger.LogError("[AuditLog] ERROR | {Summary} | {@AuditLog}",
                    auditLog.GetSummary(), auditLog);
            }
        }
    }

    /// <summary>
    /// Logs a warning audit entry
    /// </summary>
    public static void LogAuditWarning(this ILogger logger, AuditLog auditLog)
    {
        if (auditLog.Level != AuditLevel.Warning)
        {
            auditLog = auditLog with { Level = AuditLevel.Warning };
        }

        using (LogContext.PushProperty("AuditLog", auditLog, destructureObjects: true))
        using (LogContext.PushProperty("LogId", auditLog.LogId.Value))
        using (LogContext.PushProperty("CorrelationId", auditLog.CorrelationId))
        {
            logger.LogWarning("[AuditLog] WARNING | {Summary} | {@AuditLog}",
                auditLog.GetSummary(), auditLog);
        }
    }

    /// <summary>
    /// Logs a timeout audit entry with complete request/response information
    /// </summary>
    public static void LogAuditTimeout(this ILogger logger, AuditLog auditLog, Exception exception)
    {
        var timeoutLog = auditLog with
        {
            Level = AuditLevel.Error,
            ErrorCode = "RequestTimeout",
            ErrorType = "Timeout"
        };

        using (LogContext.PushProperty("AuditLog", timeoutLog, destructureObjects: true))
        using (LogContext.PushProperty("LogId", timeoutLog.LogId.Value))
        using (LogContext.PushProperty("CorrelationId", timeoutLog.CorrelationId))
        {
            logger.LogError(exception,
                "[AuditLog] TIMEOUT | {Summary} | Request: {RequestUri} | Duration: {DurationMs}ms | {@AuditLog}",
                timeoutLog.GetSummary(),
                timeoutLog.RequestUri,
                timeoutLog.DurationMs,
                timeoutLog);
        }
    }

    /// <summary>
    /// Creates a scoped logger with correlation ID
    /// </summary>
    public static IDisposable BeginAuditScope(this ILogger logger, string correlationId, LogId logId)
    {
        return LogContext.PushProperty("CorrelationId", correlationId)
            .Add(LogContext.PushProperty("LogId", logId.Value));
    }

    /// <summary>
    /// Extension method to add disposal of multiple IDisposable objects
    /// </summary>
    private static IDisposable Add(this IDisposable first, IDisposable second)
    {
        return new CompositeDisposable(first, second);
    }

    private class CompositeDisposable : IDisposable
    {
        private readonly IDisposable _first;
        private readonly IDisposable _second;

        public CompositeDisposable(IDisposable first, IDisposable second)
        {
            _first = first;
            _second = second;
        }

        public void Dispose()
        {
            _first?.Dispose();
            _second?.Dispose();
        }
    }
}
