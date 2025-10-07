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

    public void AddTimeoutLog<TBody>(HttpProviderRequest<TBody> request, Exception exception, long durationMs)
    {
        string reqString = System.Text.Json.JsonSerializer.Serialize(request);
        if (!string.IsNullOrEmpty(reqString))
        {
            reqString = MyRegex().Replace(reqString, Constants.Replaceformat);
        }
        _ = long.TryParse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);

        var callLog = new CallLogModel
        {
            RequestBody = reqString,
            ResponseBody = $"TIMEOUT after {durationMs}ms - {exception.Message}",
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = request.Uri,
            ServiceCallStatus = false,
            ServiceType = request.Service,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorCode = "RequestTimeout",
            ErrorType = "Timeout",
            ProviderType = request.Provider,
            AuditType = Enums.AuditType.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogError(exception, "[CallLog] TIMEOUT {@CallLog}", callLog);
        }
    }

    [GeneratedRegex(Constants.Pattern)]
    private static partial Regex MyRegex();
}