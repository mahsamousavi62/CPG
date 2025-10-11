using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.AuditLogAggregate;

/// <summary>
/// Represents a comprehensive audit log entry with 25 structured parameters
/// This is the main domain model for structured logging in the CPG system
/// </summary>
public sealed record AuditLog
{
    // Core Identification (3 parameters)
    /// <summary>
    /// Structured log identifier: {Id}-{Domain}-{Abbreviation}
    /// Example: "12345-Payment-IPG"
    /// </summary>
    public LogId LogId { get; init; }

    /// <summary>
    /// Correlation ID for tracing requests across services
    /// </summary>
    public string CorrelationId { get; init; }

    /// <summary>
    /// Log level (Information, Warning, Error)
    /// </summary>
    public AuditLevel Level { get; init; }

    // Service Context (4 parameters)
    /// <summary>
    /// Name of the service being called
    /// </summary>
    public string ServiceName { get; init; }

    /// <summary>
    /// Type of service (IPG, DirectDebit, etc.)
    /// </summary>
    public ServiceType? ServiceType { get; init; }

    /// <summary>
    /// Provider name (Vandar, AsanPardakht, etc.)
    /// </summary>
    public string? ProviderName { get; init; }

    /// <summary>
    /// Provider type for logging purposes
    /// </summary>
    public ProviderTypeInLog? ProviderType { get; init; }

    // User/Client Context (7 parameters)
    /// <summary>
    /// Type of audit (Client, Provider, User, Develop)
    /// </summary>
    public AuditType AuditType { get; init; }

    /// <summary>
    /// User identifier
    /// </summary>
    public long? UserId { get; init; }

    /// <summary>
    /// Client IP address
    /// </summary>
    public string? Ip { get; init; }

    /// <summary>
    /// User agent string from HTTP headers
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// Application/client identifier
    /// </summary>
    public long? ApplicationId { get; init; }

    /// <summary>
    /// Company identifier
    /// </summary>
    public long? CompanyId { get; init; }

    /// <summary>
    /// Client ID (OAuth/IDP client identifier)
    /// </summary>
    public string? ClientId { get; init; }

    // HTTP Request/Response (6 parameters)
    /// <summary>
    /// HTTP request URI
    /// </summary>
    public string? RequestUri { get; init; }

    /// <summary>
    /// HTTP request headers (JSON serialized)
    /// </summary>
    public string? RequestHeader { get; init; }

    /// <summary>
    /// HTTP request body (may be truncated)
    /// </summary>
    public string? RequestBody { get; init; }

    /// <summary>
    /// HTTP response status code
    /// </summary>
    public int? ResponseStatusCode { get; init; }

    /// <summary>
    /// HTTP response headers (JSON serialized)
    /// </summary>
    public string? ResponseHeader { get; init; }

    /// <summary>
    /// HTTP response body (may be truncated)
    /// </summary>
    public string? ResponseBody { get; init; }

    // Timing (3 parameters)
    /// <summary>
    /// Start timestamp of the operation
    /// </summary>
    public DateTime StartDateTime { get; init; }

    /// <summary>
    /// End timestamp of the operation
    /// </summary>
    public DateTime EndDateTime { get; init; }

    /// <summary>
    /// Duration of the operation in milliseconds
    /// </summary>
    public long DurationMs { get; init; }

    // Error Information (2 parameters)
    /// <summary>
    /// Error code (HTTP status code, business error code, etc.)
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Error type or category
    /// </summary>
    public string? ErrorType { get; init; }

    private AuditLog()
    {
    }

    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    public AuditLog(
        LogId logId,
        string correlationId,
        AuditLevel level,
        string serviceName,
        AuditType auditType,
        DateTime startDateTime,
        DateTime endDateTime,
        long durationMs,
        ServiceType? serviceType = null,
        string? providerName = null,
        ProviderTypeInLog? providerType = null,
        long? userId = null,
        string? ip = null,
        string? userAgent = null,
        long? applicationId = null,
        long? companyId = null,
        string? clientId = null,
        string? requestUri = null,
        string? requestHeader = null,
        string? requestBody = null,
        int? responseStatusCode = null,
        string? responseHeader = null,
        string? responseBody = null,
        string? errorCode = null,
        string? errorType = null)
    {
        LogId = logId ?? throw new ArgumentNullException(nameof(logId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Level = level;
        ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        ServiceType = serviceType;
        ProviderName = providerName;
        ProviderType = providerType;
        AuditType = auditType;
        UserId = userId;
        Ip = ip;
        UserAgent = userAgent;
        ApplicationId = applicationId;
        CompanyId = companyId;
        ClientId = clientId;
        RequestUri = requestUri;
        RequestHeader = requestHeader;
        RequestBody = requestBody;
        ResponseStatusCode = responseStatusCode;
        ResponseHeader = responseHeader;
        ResponseBody = responseBody;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        DurationMs = durationMs;
        ErrorCode = errorCode;
        ErrorType = errorType;
    }

    /// <summary>
    /// Indicates whether this log represents a successful operation
    /// </summary>
    public bool IsSuccess => Level == AuditLevel.Information && string.IsNullOrEmpty(ErrorCode);

    /// <summary>
    /// Indicates whether this log represents an error
    /// </summary>
    public bool IsError => Level == AuditLevel.Error;

    /// <summary>
    /// Gets a summary description for this audit log
    /// </summary>
    public string GetSummary()
    {
        var status = IsSuccess ? "SUCCESS" : (IsError ? "ERROR" : "WARNING");
        return $"[{LogId}] {ServiceName} - {status} ({DurationMs}ms)";
    }
}
