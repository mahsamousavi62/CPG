using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using HotChocolate.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
//using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Logging;

public class LogService(ILogger<LogService> logger , IHttpContextAccessor httpContextAccessor) : ILogService 
{
  
    private readonly ILogger<LogService> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string ServiceName { get; set; }
    public Enums.ServiceType ServiceType { get; set; }
    public Enums.ProviderType ProviderType{ get; set; }

    public void AddServiceCallLog<TBody>(HttpProviderRequest<TBody> request, HttpResponseMessage response, string resString)
    {
        if (!string.IsNullOrEmpty(resString))
        {
            resString = Regex.Replace(resString, Constants.Pattern, Constants.Replaceformat);
        }
        string reqString = System.Text.Json.JsonSerializer.Serialize(request);
        if (!string.IsNullOrEmpty(reqString))
        {
            reqString = Regex.Replace(reqString, Constants.Pattern, Constants.Replaceformat);
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
            AuditType=Enums.AuditType.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }

public void AddServiceCallLog(string request,string response,short status,string message)
    {
        _ = long.TryParse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);

        var callLog = new CallLogModel
        {
            RequestBody = request,
            ResponseBody = response,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = ServiceName,
            ServiceCallStatus = status == 0 ? true : false,
            ServiceType = ServiceType,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorCode = status<0 ? message:"",
            ErrorType = status < 0 ? status.ToString() : "",
            ProviderType = ProviderType,
            AuditType = Enums.AuditType.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }
}