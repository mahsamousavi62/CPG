using System;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

/// <summary>
/// Structured audit logging service for CPG system
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Log client API calls (incoming requests from OAuth clients/applications)
    /// </summary>
    void LogClientCall(ClientCallLog log);

    /// <summary>
    /// Log provider calls (outgoing HTTP/SOAP calls to external payment providers)
    /// </summary>
    void LogProviderCall(ProviderCallLog log);

    /// <summary>
    /// Log user actions (authenticated user operations)
    /// </summary>
    void LogUserAction(UserActionLog log);
}

/// <summary>
/// Log model for client API calls
/// </summary>
public class ClientCallLog
{
    public required string ServiceName { get; set; }
    public required string HttpMethod { get; set; }
    public required string RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public required int StatusCode { get; set; }
    public required bool IsSuccess { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required long DurationMs { get; set; }

    // Client context
    public long? ApplicationId { get; set; }
    public string? ClientId { get; set; }
    public long? CompanyId { get; set; }

    // User context
    public long? UserId { get; set; }
    public string? MobilePhone { get; set; }

    // Request context
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}

/// <summary>
/// Log model for provider calls (HTTP/SOAP)
/// </summary>
public class ProviderCallLog
{
    public required string ProviderName { get; set; }
    public required ProviderTypeInLog ProviderType { get; set; }
    public required ServiceType ServiceType { get; set; }
    public required string ServiceUrl { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public required bool IsSuccess { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required long DurationMs { get; set; }

    // Timeout/Retry context
    public bool IsTimeout { get; set; }
    public int? RetryAttempt { get; set; }

    // User context (who initiated the call)
    public long? UserId { get; set; }
    public long? CompanyId { get; set; }
    public long? ApplicationId { get; set; }

    // Request context
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}

/// <summary>
/// Log model for user actions
/// </summary>
public class UserActionLog
{
    public required string ActionName { get; set; }
    public required string ActionType { get; set; } // Create, Update, Delete, Read
    public required long UserId { get; set; }
    public long? CompanyId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public Dictionary<string, object>? Changes { get; set; }
    public required bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime Timestamp { get; set; }

    // Request context
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}
