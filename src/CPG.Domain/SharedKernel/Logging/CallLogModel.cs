using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

/// <summary>
/// Comprehensive log model for all service calls (Client, Provider, User)
/// Based on task.md specifications with all 25 required parameters
/// </summary>
public class CallLogModel
{
    // ===== Core Identifiers (از task.md) =====

    /// <summary>
    /// شناسه زنجیره زیر درخواست - Unique identifier for sub-request chain
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// شناسه تصادفی - Random unique ID for each log entry
    /// Format: {Id}-{Domain}-{Abbreviation of Exception}
    /// </summary>
    public string? LogId { get; set; }

    /// <summary>
    /// شناسه زنجیره - Unique identifier for entire request chain
    /// </summary>
    public string? RequestId { get; set; }

    // ===== Audit Categorization (از task.md) =====

    /// <summary>
    /// سطح رویداد - Log level: Information, Warning, Error
    /// </summary>
    public string? AuditLevel { get; set; }

    /// <summary>
    /// نوع رویداد - Audit type: Client, Provider, User
    /// </summary>
    public AuditType? AuditType { get; set; }

    // ===== Service Information (از task.md) =====

    /// <summary>
    /// نام سرویس - Service or API name that was called
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// عنوان سرویس دهنده - Provider name (from Provider.latin_name)
    /// </summary>
    public string? ProviderName { get; set; }

    /// <summary>
    /// نوع سرویس دهنده
    /// </summary>
    public ProviderTypeInLog? ProviderType { get; set; }

    /// <summary>
    /// نوع سرویس - Http, Soap, etc.
    /// </summary>
    public ServiceType? ServiceType { get; set; }

    // ===== Request Information (از task.md) =====

    /// <summary>
    /// uri درخواست - Request URI
    /// </summary>
    public string? RequestUri { get; set; }

    /// <summary>
    /// هدر درخواست - Request headers
    /// </summary>
    public string? RequestHeader { get; set; }

    /// <summary>
    /// بدنه درخواست - Request body
    /// </summary>
    public string? RequestBody { get; set; }

    // ===== Response Information (از task.md) =====

    /// <summary>
    /// کد وضعیت نتیجه - HTTP status code or custom status
    /// </summary>
    public int? ResponseStatusCode { get; set; }

    /// <summary>
    /// هدر نتیجه - Response headers
    /// </summary>
    public string? ResponseHeader { get; set; }

    /// <summary>
    /// بدنه نتیجه - Response body
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// نتیجه بازگشتی به کاربر - User-facing response message
    /// </summary>
    public string? Response { get; set; }

    // ===== Error Information (از task.md) =====

    /// <summary>
    /// کد خطا - Categorized error code
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// نوع خطا - Error type or exception name
    /// </summary>
    public string? ErrorType { get; set; }

    /// <summary>
    /// استک تریس - Stack trace for unhandled exceptions
    /// </summary>
    public string? StackTrace { get; set; }

    // ===== Context Information (از task.md) =====

    /// <summary>
    /// شناسه برنامه - Application ID from OAuth client
    /// </summary>
    public long? ApplicationId { get; set; }

    /// <summary>
    /// شناسه کاربر - User ID if authenticated
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// شناسه شرکت - Company ID for company-related operations
    /// </summary>
    public long? CompanyId { get; set; }

    /// <summary>
    /// ip کاربر - Client IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// نماینده کاربر - User agent string from browser/client
    /// </summary>
    public string? UserAgent { get; set; }

    // ===== Status & Timing (از task.md) =====

    /// <summary>
    /// وضعیت موفقیت - Success status: true (1) = success, false (0) = failure
    /// </summary>
    public bool IsSucceeded { get; set; }

    /// <summary>
    /// تاریخ و زمان شروع - Start date/time of operation
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// تاریخ و زمان پایان - End date/time of operation
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// مدت زمان انجام عملیات بر حسب میلی ثانیه - Duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    // ===== Legacy/Compatibility Fields =====

    /// <summary>
    /// Legacy field - mapped to IsSucceeded
    /// </summary>
    [Obsolete("Use IsSucceeded instead")]
    public bool? ServiceCallStatus
    {
        get => IsSucceeded;
        set => IsSucceeded = value ?? false;
    }

    /// <summary>
    /// Legacy field - mapped to RequestUri
    /// </summary>
    [Obsolete("Use RequestUri instead")]
    public string? ServiceCallUrl
    {
        get => RequestUri;
        set => RequestUri = value;
    }

    /// <summary>
    /// Legacy field - mapped to StartDateTime
    /// </summary>
    [Obsolete("Use StartDateTime instead")]
    public DateTime ServiceCallDate
    {
        get => StartDateTime;
        set => StartDateTime = value;
    }

    /// <summary>
    /// Legacy field - mapped to StartDateTime
    /// </summary>
    [Obsolete("Use StartDateTime instead")]
    public DateTime CreationDate
    {
        get => StartDateTime;
        set => StartDateTime = value;
    }

    /// <summary>
    /// Legacy field - mapped to UserId
    /// </summary>
    [Obsolete("Use UserId instead")]
    public long CreationUserId
    {
        get => UserId ?? 0;
        set => UserId = value > 0 ? value : null;
    }

    /// <summary>
    /// Legacy typo - use CorrelationId instead
    /// </summary>
    [Obsolete("Typo - use CorrelationId instead")]
    public string? CorrolationId
    {
        get => CorrelationId;
        set => CorrelationId = value;
    }
}
