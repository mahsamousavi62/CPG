using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CPG.Infrastructure.Logging;

/// <summary>
/// Structured audit logging service implementation
/// Provides consistent logging format across client calls, provider calls, and user actions
/// </summary>
public partial class AuditLogService : IAuditLogService
{
    private readonly ILogger<AuditLogService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(ILogger<AuditLogService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public void LogClientCall(ClientCallLog log)
    {
        // Sanitize sensitive data
        log.RequestBody = SanitizeSensitiveData(log.RequestBody);
        log.ResponseBody = SanitizeSensitiveData(log.ResponseBody);

        // Enrich with context from HttpContext if not provided
        if (_httpContextAccessor.HttpContext != null)
        {
            log.CorrelationId ??= _httpContextAccessor.HttpContext.TraceIdentifier;
            log.IpAddress ??= _httpContextAccessor.HttpContext.Request.GetClientIpAddress();
            log.UserAgent ??= _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        }

        // Use structured logging with Serilog
        using (LogContext.PushProperty("AuditType", "ClientCall", false))
        using (LogContext.PushProperty("ClientCallLog", log, true))
        {
            if (log.IsSuccess)
            {
                _logger.LogInformation(
                    "[ClientCall] {ServiceName} {HttpMethod} {RequestPath} - Status: {StatusCode}, Duration: {DurationMs}ms",
                    log.ServiceName,
                    log.HttpMethod,
                    log.RequestPath,
                    log.StatusCode,
                    log.DurationMs);
            }
            else
            {
                _logger.LogWarning(
                    "[ClientCall] {ServiceName} {HttpMethod} {RequestPath} - Status: {StatusCode}, Duration: {DurationMs}ms, Error: {ErrorCode} - {ErrorMessage}",
                    log.ServiceName,
                    log.HttpMethod,
                    log.RequestPath,
                    log.StatusCode,
                    log.DurationMs,
                    log.ErrorCode,
                    log.ErrorMessage);
            }
        }
    }

    public void LogProviderCall(ProviderCallLog log)
    {
        // Sanitize sensitive data
        log.RequestBody = SanitizeSensitiveData(log.RequestBody);
        log.ResponseBody = SanitizeSensitiveData(log.ResponseBody);

        // Enrich with context from HttpContext if not provided
        if (_httpContextAccessor.HttpContext != null)
        {
            log.CorrelationId ??= _httpContextAccessor.HttpContext.TraceIdentifier;
        }

        // Use structured logging with Serilog
        using (LogContext.PushProperty("AuditType", "Provider", false))
        using (LogContext.PushProperty("ProviderCallLog", log, true))
        {
            if (log.IsTimeout)
            {
                _logger.LogError(
                    "[ProviderCall] TIMEOUT - {ProviderName} ({ProviderType}) {ServiceType} - Duration: {DurationMs}ms, URL: {ServiceUrl}, Retry: {RetryAttempt}",
                    log.ProviderName,
                    log.ProviderType,
                    log.ServiceType,
                    log.DurationMs,
                    log.ServiceUrl,
                    log.RetryAttempt ?? 0);
            }
            else if (log.IsSuccess)
            {
                _logger.LogInformation(
                    "[ProviderCall] {ProviderName} ({ProviderType}) {ServiceType} - Duration: {DurationMs}ms, URL: {ServiceUrl}",
                    log.ProviderName,
                    log.ProviderType,
                    log.ServiceType,
                    log.DurationMs,
                    log.ServiceUrl);
            }
            else
            {
                _logger.LogWarning(
                    "[ProviderCall] FAILED - {ProviderName} ({ProviderType}) {ServiceType} - Duration: {DurationMs}ms, URL: {ServiceUrl}, Error: {ErrorCode} - {ErrorMessage}",
                    log.ProviderName,
                    log.ProviderType,
                    log.ServiceType,
                    log.DurationMs,
                    log.ServiceUrl,
                    log.ErrorCode,
                    log.ErrorMessage);
            }
        }
    }

    public void LogUserAction(UserActionLog log)
    {
        // Enrich with context from HttpContext if not provided
        if (_httpContextAccessor.HttpContext != null)
        {
            log.CorrelationId ??= _httpContextAccessor.HttpContext.TraceIdentifier;
            log.IpAddress ??= _httpContextAccessor.HttpContext.Request.GetClientIpAddress();
        }

        // Use structured logging with Serilog
        using (LogContext.PushProperty("AuditType", "User", false))
        using (LogContext.PushProperty("UserActionLog", log, true))
        {
            if (log.IsSuccess)
            {
                _logger.LogInformation(
                    "[UserAction] {ActionType} {ActionName} by User {UserId} on {EntityType} {EntityId}",
                    log.ActionType,
                    log.ActionName,
                    log.UserId,
                    log.EntityType ?? "Unknown",
                    log.EntityId ?? "N/A");
            }
            else
            {
                _logger.LogWarning(
                    "[UserAction] FAILED - {ActionType} {ActionName} by User {UserId} on {EntityType} {EntityId} - Error: {ErrorMessage}",
                    log.ActionType,
                    log.ActionName,
                    log.UserId,
                    log.EntityType ?? "Unknown",
                    log.EntityId ?? "N/A",
                    log.ErrorMessage);
            }
        }
    }

    /// <summary>
    /// Sanitize sensitive data from request/response bodies
    /// Masks credit card numbers, passwords, tokens, etc.
    /// </summary>
    private string? SanitizeSensitiveData(string? data)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        try
        {
            // Use the regex pattern from Constants if available
            return SensitiveDataRegex().Replace(data, Constants.Replaceformat);
        }
        catch
        {
            return data;
        }
    }

    [GeneratedRegex(Constants.Pattern)]
    private static partial Regex SensitiveDataRegex();
}
