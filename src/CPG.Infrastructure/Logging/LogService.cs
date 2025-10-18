using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Linq;
using System.Net.Http;
using System.Reactive.Joins;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Logging;

public partial class LogService(ILogger<LogService> logger, IHttpContextAccessor httpContextAccessor) : ILogService
{

    private readonly ILogger<LogService> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderTypeInLog ProviderTypeInLog { get; set; }

    public void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString)
    {
        if (!string.IsNullOrEmpty(resString))
        {
            resString = MyRegex().Replace(resString, Constants.Replaceformat);
        }
        string reqString = System.Text.Json.JsonSerializer.Serialize(request);
        if (!string.IsNullOrEmpty(reqString))
        {
            reqString = MyRegex().Replace(reqString, Constants.Replaceformat);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callLog = response.IsSuccessStatusCode
            ? CallLogModel.CreateSuccess(
                serviceName: request.Service?.ToString() ?? "Unknown",
                providerName: request.Provider?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                serviceType: request.Service,
                providerType: request.Provider,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: (int)response.StatusCode
            )
            : CallLogModel.CreateError(
                serviceName: request.Service?.ToString() ?? "Unknown",
                providerName: request.Provider?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                exception: null,
                serviceType: request.Service,
                providerType: request.Provider,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    public void AddServiceCallLog(string request, string response, short status, string message)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callLog = status == 0
            ? CallLogModel.CreateSuccess(
                serviceName: ServiceName ?? "Unknown",
                providerName: ProviderTypeInLog?.ToString() ?? "Unknown",
                requestUri: ServiceName,
                requestBody: request,
                responseBody: response,
                serviceType: ServiceType,
                providerType: ProviderTypeInLog,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: 200
            )
            : CallLogModel.CreateError(
                serviceName: ServiceName ?? "Unknown",
                providerName: ProviderTypeInLog?.ToString() ?? "Unknown",
                requestUri: ServiceName,
                requestBody: request,
                responseBody: response,
                exception: null,
                serviceType: ServiceType,
                providerType: ProviderTypeInLog,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    public async Task AddServiceCallLogAsync<TBody, TRequest>(HttpProviderRequest<TBody, TRequest> request, HttpResponseMessage response)
    {
        string resString = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(resString))
        {
            resString = MyRegex().Replace(resString, Constants.Replaceformat);
        }
        string reqString = System.Text.Json.JsonSerializer.Serialize(request);
        if (!string.IsNullOrEmpty(reqString))
        {
            reqString = MyRegex().Replace(reqString, Constants.Replaceformat);
        }

        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callLog = response.IsSuccessStatusCode
            ? CallLogModel.CreateSuccess(
                serviceName: request.Service?.ToString() ?? "Unknown",
                providerName: request.ProviderTypeInLog?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                serviceType: request.Service,
                providerType: request.ProviderTypeInLog,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: (int)response.StatusCode
            )
            : CallLogModel.CreateError(
                serviceName: request.Service?.ToString() ?? "Unknown",
                providerName: request.ProviderTypeInLog?.ToString() ?? "Unknown",
                requestUri: request.Uri,
                requestBody: reqString,
                responseBody: resString,
                exception: null,
                serviceType: request.Service,
                providerType: request.ProviderTypeInLog,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: userId == 0 ? 1 : userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    [GeneratedRegex("(usr|pwd|merchantConfigurationId|key|iv|userPassword)\\\"\\s*(:)\\s*\"([^\"]*)\"")]
    private static partial Regex MyRegex();

    // New structured logging methods
    public void LogInformation(CallLogModel callLog)
    {
        PrepareCallLog(callLog, 1); // 1 = Information
        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    public void LogWarning(CallLogModel callLog)
    {
        PrepareCallLog(callLog, 2); // 2 = Warning
        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogWarning("[CallLog] {@CallLog}", callLog);
        }
    }

    public void LogError(CallLogModel callLog)
    {
        PrepareCallLog(callLog, 3); // 3 = Error
        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogError("[CallLog] {@CallLog}", callLog);
        }
    }

    public void LogDebug(CallLogModel callLog)
    {
        PrepareCallLog(callLog, 4); // 4 = Debug
        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogDebug("[CallLog] {@CallLog}", callLog);
        }
    }

    private void PrepareCallLog(CallLogModel callLog, int auditLevel)
    {
        // Set audit level if not already set
        if (callLog.AuditLevel == 0)
        {
            callLog.AuditLevel = auditLevel;
        }

        // Generate LogId if not set: {Id} - {Domain} - {Abbreviation of Exception}
        if (string.IsNullOrEmpty(callLog.LogId))
        {
            var domain = callLog.ServiceName ?? "CPG";
            var exceptionAbbr = GetExceptionAbbreviation(callLog.ErrorCode);
            callLog.LogId = $"{Guid.NewGuid()} - {domain} - {exceptionAbbr}";
        }

        // Set EndDateTime and calculate Duration if not set
        if (!callLog.EndDateTime.HasValue && callLog.StartDateTime != default)
        {
            callLog.EndDateTime = DateTime.Now;
            callLog.DurationMs = (long)(callLog.EndDateTime.Value - callLog.StartDateTime).TotalMilliseconds;
        }

        // Get correlation ID and request ID from HTTP context if not set
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            if (string.IsNullOrEmpty(callLog.CorrelationId))
            {
                callLog.CorrelationId = httpContext.TraceIdentifier;
            }

            if (string.IsNullOrEmpty(callLog.RequestId))
            {
                callLog.RequestId = httpContext.TraceIdentifier;
            }

            if (string.IsNullOrEmpty(callLog.Ip))
            {
                callLog.Ip = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrEmpty(callLog.UserAgent))
            {
                callLog.UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            }
        }
    }

    private static string GetExceptionAbbreviation(string errorCode)
    {
        if (string.IsNullOrEmpty(errorCode))
            return "OK";

        // Extract abbreviation from exception type name
        // e.g., "NullReferenceException" -> "NRE"
        var words = System.Text.RegularExpressions.Regex.Split(errorCode, @"(?<!^)(?=[A-Z])");
        var abbreviation = string.Join("", words.Select(w => w.Length > 0 ? w[0] : ' ').Take(3));
        return abbreviation.ToUpper();
    }
}