using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

public class CallLogModel
{
    // 1. correlation_id - شناسه زنجیره زیر درخواست
    public string CorrelationId { get; set; }

    // 2. log_id - شناسه تصادفی (Format: {Id} - {Domain} - {Abbreviation of Exception})
    public string LogId { get; set; }

    // 3. request_id - شناسه زنجیره کل درخواست
    public string RequestId { get; set; }

    // 4. audit_level - سطح رویداد (1-information, 2-warning, 3-error)
    public int AuditLevel { get; set; }

    // 5. audit_type - نوع رویداد (client/provider/user)
    public AuditType? AuditType { get; set; }

    // 6. service_name - نام سرویس
    public string ServiceName { get; set; }

    // 7. provider_name - عنوان سرویس دهنده
    public string ProviderName { get; set; }

    // 8. request_uri - URI درخواست
    public string RequestUri { get; set; }

    // 9. request_header - هدر درخواست
    public string RequestHeader { get; set; }

    // 10. request_body - بدنه درخواست
    public string RequestBody { get; set; }

    // 11. response_status_code - کد وضعیت نتیجه
    public int? ResponseStatusCode { get; set; }

    // 12. response_header - هدر نتیجه
    public string ResponseHeader { get; set; }

    // 13. response_body - بدنه نتیجه
    public string ResponseBody { get; set; }

    // 14. application_id - شناسه برنامه
    public long? ApplicationId { get; set; }

    // 15. user_id - شناسه کاربر
    public long? UserId { get; set; }

    // 16. ip - IP کاربر یا سرور
    public string Ip { get; set; }

    // 17. company_id - شناسه شرکت
    public long? CompanyId { get; set; }

    // 18. user_agent - نماینده کاربر
    public string UserAgent { get; set; }

    // 19. response - نتیجه بازگشتی به کاربر
    public string Response { get; set; }

    // 20. error_code - کد خطا
    public string ErrorCode { get; set; }

    // 21. is_succeeded - وضعیت موفقیت (1/0)
    public bool IsSucceeded { get; set; }

    // 22. start_date_time - تاریخ و زمان شروع
    public DateTime StartDateTime { get; set; }

    // 23. end_date_time - تاریخ و زمان پایان
    public DateTime? EndDateTime { get; set; }

    // 24. duration_ms - مدت زمان به میلی ثانیه
    public long? DurationMs { get; set; }

    // 25. stack_trace - استک تریس
    public string StackTrace { get; set; }

    // Additional fields for backwards compatibility and enums
    public ServiceType? ServiceType { get; set; }
    public ProviderTypeInLog? ProviderType { get; set; }
    public string ErrorType { get; set; }

    /// <summary>
    /// Factory method to create CallLogModel with comprehensive audit fields
    /// </summary>
    public static CallLogModel CreateError(
        string serviceName,
        string providerName,
        string requestUri,
        string requestBody,
        string responseBody,
        Exception exception = null,
        ServiceType? serviceType = null,
        ProviderTypeInLog? providerType = null,
        AuditType? auditType = null,
        string correlationId = null,
        long? userId = null,
        long? applicationId = null,
        long? companyId = null,
        string ip = null,
        string userAgent = null)
    {
        var errorTime = DateTime.Now;
        return new CallLogModel
        {
            // New fields (25 required fields)
            CorrelationId = correlationId,
            LogId = $"{Guid.NewGuid()} - {serviceName} - {exception?.GetType().Name ?? "Error"}",
            RequestId = correlationId,
            AuditLevel = 3, // Error level
            AuditType = auditType ?? AuditType.Provider,
            ServiceName = serviceName,
            ProviderName = providerName,
            RequestUri = requestUri,
            RequestHeader = null,
            RequestBody = requestBody,
            ResponseStatusCode = 500,
            ResponseHeader = null,
            ResponseBody = responseBody,
            ApplicationId = applicationId == 0 ? null : applicationId,
            UserId = userId == 0 ? null : userId,
            Ip = ip,
            CompanyId = companyId == 0 ? null : companyId,
            UserAgent = userAgent,
            Response = responseBody,
            ErrorCode = exception?.GetType().Name,
            IsSucceeded = false,
            StartDateTime = errorTime,
            EndDateTime = errorTime,
            DurationMs = 0,
            StackTrace = exception?.StackTrace,

            // Additional fields
            ServiceType = serviceType,
            ProviderType = providerType,
            ErrorType = exception?.Message ?? responseBody
        };
    }

    /// <summary>
    /// Factory method to create CallLogModel for successful operations
    /// </summary>
    public static CallLogModel CreateSuccess(
        string serviceName,
        string providerName,
        string requestUri,
        string requestBody,
        string responseBody,
        ServiceType? serviceType = null,
        ProviderTypeInLog? providerType = null,
        AuditType? auditType = null,
        string correlationId = null,
        long? userId = null,
        long? applicationId = null,
        long? companyId = null,
        string ip = null,
        string userAgent = null,
        int? responseStatusCode = null)
    {
        var callTime = DateTime.Now;
        return new CallLogModel
        {
            // New fields (25 required fields)
            CorrelationId = correlationId,
            LogId = $"{Guid.NewGuid()} - {serviceName} - Success",
            RequestId = correlationId,
            AuditLevel = 1, // Information level
            AuditType = auditType ?? AuditType.Provider,
            ServiceName = serviceName,
            ProviderName = providerName,
            RequestUri = requestUri,
            RequestHeader = null,
            RequestBody = requestBody,
            ResponseStatusCode = responseStatusCode ?? 200,
            ResponseHeader = null,
            ResponseBody = responseBody,
            ApplicationId = applicationId == 0 ? null : applicationId,
            UserId = userId == 0 ? null : userId,
            Ip = ip,
            CompanyId = companyId == 0 ? null : companyId,
            UserAgent = userAgent,
            Response = responseBody,
            ErrorCode = null,
            IsSucceeded = true,
            StartDateTime = callTime,
            EndDateTime = callTime,
            DurationMs = 0,
            StackTrace = null,

            // Additional fields
            ServiceType = serviceType,
            ProviderType = providerType,
            ErrorType = null
        };
    }
}
