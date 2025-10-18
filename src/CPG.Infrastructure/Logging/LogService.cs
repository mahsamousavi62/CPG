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
        _ = long.TryParse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);

        var callLog = new CallLogModel
        {
            RequestBody = reqString,
            ResponseBody = resString,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = request.Uri,
            ServiceCallStatus = response.StatusCode == System.Net.HttpStatusCode.OK,
            ServiceType = request.Service,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
            ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
            ProviderType = request.Provider,
            AuditType = Enums.AuditType.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    public void AddServiceCallLog(string request, string response, short status, string message)
    {
        _ = long.TryParse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);

        var callLog = new CallLogModel
        {
            RequestBody = request,
            ResponseBody = response,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = ServiceName,
            ServiceCallStatus = status == 0,
            ServiceType = ServiceType,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorCode = status < 0 ? message : "",
            ErrorType = status < 0 ? status.ToString() : "",
            ProviderType = ProviderTypeInLog,
            AuditType = Enums.AuditType.Provider
        };

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
        _ = long.TryParse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);
        
        var callLog = new CallLogModel
        {
            RequestBody = reqString,
            ResponseBody = resString,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = request.Uri,
            ServiceCallStatus = response.StatusCode == System.Net.HttpStatusCode.OK,
            ServiceType = request.Service,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
            ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
            ProviderType = request.ProviderTypeInLog,
            AuditType = Enums.AuditType.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

    // متد جدید: لاگ timeout با جزئیات کامل (S004)
    public void LogTimeout(string serviceName, string operationName, TimeSpan duration,
        string requestBody, string partialResponse)
    {
        var timeoutLog = new
        {
            ServiceName = serviceName,
            OperationName = operationName,
            DurationMs = (long)duration.TotalMilliseconds,
            RequestBody = TruncateBody(MaskSensitiveData(requestBody), 3000),
            PartialResponseBody = TruncateBody(MaskSensitiveData(partialResponse), 2000),
            Timestamp = DateTime.UtcNow,
            RequestId = GetCorrelationId()
        };

        using (LogContext.PushProperty("Timeout", timeoutLog, true))
        {
            _logger.LogError(
                "[TIMEOUT] {ServiceName}.{OperationName} after {DurationMs}ms | RequestId: {RequestId}",
                serviceName, operationName, timeoutLog.DurationMs, timeoutLog.RequestId);
        }
    }

    // متد جدید: برش بدنه برای جلوگیری از لاگ زیاد (S005)
    public string TruncateBody(string body, int maxLength)
    {
        if (string.IsNullOrEmpty(body) || body.Length <= maxLength)
            return body;

        return body.Substring(0, maxLength) + $"... [بریده: {body.Length - maxLength} کاراکتر]";
    }

    // متد جدید: گرفتن Correlation ID از HttpContext (S006)
    private string GetCorrelationId()
    {
        return _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
    }

    // توسعه متد masking: PCI-DSS compliance (S007)
    private string MaskSensitiveData(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // استفاده از regex موجود
        content = MyRegex().Replace(content, Constants.Replaceformat);

        // اضافه کردن masking برای PAN (نگه داشتن 4 رقم آخر)
        content = Regex.Replace(
            content, @"\b(\d{12})(\d{4})\b", "************$2");

        // CVV2 کامل پاک شود
        content = Regex.Replace(
            content, @"""cvv2?""\s*:\s*""\d{3,4}""", "\"cvv2\":\"***\"",
            RegexOptions.IgnoreCase);

        // Token ها رو به 10 کاراکتر اول برش بزن
        content = Regex.Replace(
            content, @"""(token|access_token)""\s*:\s*""([^""]{10})[^""]*""",
            "\"$1\":\"$2...\"", RegexOptions.IgnoreCase);

        return content;
    }

    [GeneratedRegex(Constants.Pattern)]
    private static partial Regex MyRegex();
}