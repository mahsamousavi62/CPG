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

    // Original fields
    public ServiceType? ServiceType { get; set; }
    public ProviderTypeInLog? ProviderType { get; set; }
    public bool? ServiceCallStatus { get; set; }
    public string ServiceCallUrl { get; set; }
    public DateTime ServiceCallDate { get; set; }
    public DateTime CreationDate { get; set; }
    public long CreationUserId { get; set; }
    public string CorrolationId { get; set; }
    public string ErrorType { get; set; }
}
