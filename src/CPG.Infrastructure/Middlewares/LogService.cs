using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Helper.CallLog;
using CPG.Domain.SharedKernel.Helper.ServiceLog;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
//using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Middlewares;

public class LogService(ILogger<LogService> logger) : ILogService
{
  
    private readonly ILogger<LogService> _logger = logger;

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

        var callLog = new CallLogModel
        {
            RequestBody = reqString,
            ResponseBody = resString,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = request.Uri,
            ServiceCallStatus = response.StatusCode == System.Net.HttpStatusCode.OK,
            ServiceType = request.Service,
            CreationDate = DateTime.Now,
            CreationUserId = 1,
            ErrorCode = response.IsSuccessStatusCode ? null : ReasonPhrases.GetReasonPhrase((int)response.StatusCode),
            ErrorType = response.IsSuccessStatusCode ? null : response.StatusCode.ToString(),
            ProviderType = request.Provider
        };

        using (LogContext.PushProperty("CallLog", callLog, true))
        {
            _logger.LogInformation("[CallLog] {@CallLog}", callLog);
        }
    }
}