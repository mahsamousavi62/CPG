using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Logging;

public class CallLogModel
{

    public string CorrelationId { get; set; }


    public string LogId { get; set; }


    public string RequestId { get; set; }


    public int AuditLevel { get; set; }


    public AuditType? AuditType { get; set; }


    public string ServiceName { get; set; }


    public string ProviderName { get; set; }


    public string RequestUri { get; set; }


    public string RequestHeader { get; set; }


    public string RequestBody { get; set; }


    public int? ResponseStatusCode { get; set; }


    public string ResponseHeader { get; set; }


    public string ResponseBody { get; set; }


    public long? ApplicationId { get; set; }


    public long? UserId { get; set; }


    public string Ip { get; set; }


    public long? CompanyId { get; set; }


    public string UserAgent { get; set; }


    public string Response { get; set; }


    public string ErrorCode { get; set; }


    public bool IsSucceeded { get; set; }


    public DateTime StartDateTime { get; set; }


    public DateTime? EndDateTime { get; set; }


    public long? DurationMs { get; set; }


    public string StackTrace { get; set; }


    public ServiceType? ServiceType { get; set; }
    public ProviderTypeInLog? ProviderType { get; set; }
    public string ErrorType { get; set; }

    /// <summary>
    /// Number of retry attempts made by Polly retry policy (0 = no retry, 1+ = retried)
    /// </summary>
    public int? RetryCount { get; set; }




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
        string userAgent = null,
        string requestHeader = null,
        string responseHeader = null)
    {
        var errorTime = DateTime.Now;
        return new CallLogModel
        {

            CorrelationId = correlationId,
            LogId = $"{Guid.NewGuid()} - {serviceName} - {exception?.GetType().Name ?? "Error"}",
            RequestId = correlationId,
            AuditLevel = 3,
            AuditType = auditType ?? Enums.AuditType.Provider,
            ServiceName = serviceName,
            ProviderName = providerName,
            RequestUri = requestUri,
            RequestHeader = requestHeader,
            RequestBody = requestBody,
            ResponseStatusCode = 500,
            ResponseHeader = responseHeader,
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


            ServiceType = serviceType,
            ProviderType = providerType,
            ErrorType = exception?.Message ?? responseBody
        };
    }




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
        int? responseStatusCode = null,
        string requestHeader = null,
        string responseHeader = null)
    {
        var callTime = DateTime.Now;
        return new CallLogModel
        {

            CorrelationId = correlationId,
            LogId = $"{Guid.NewGuid()} - {serviceName} - Success",
            RequestId = correlationId,
            AuditLevel = 1,
            AuditType = auditType ?? Enums.AuditType.Provider,
            ServiceName = serviceName,
            ProviderName = providerName,
            RequestUri = requestUri,
            RequestHeader = requestHeader,
            RequestBody = requestBody,
            ResponseStatusCode = responseStatusCode ?? 200,
            ResponseHeader = responseHeader,
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


            ServiceType = serviceType,
            ProviderType = providerType,
            ErrorType = null
        };
    }
}
