using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

/// <summary>
/// Log model for provider call operations (HTTP/SOAP)
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
    public bool IsTimeout { get; set; }

    // User context
    public long? UserId { get; set; }
    public long? CompanyId { get; set; }
    public long? ApplicationId { get; set; }

    // Request context
    public string? CorrelationId { get; set; }
    public string? RequestId { get; set; }
}
