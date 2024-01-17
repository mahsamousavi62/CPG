using Azure;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Serilog.Context;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Logging;

public class LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private readonly RequestDelegate next = next;
    private readonly ILogger logger = loggerFactory.CreateLogger<LoggingMiddleware>();
    private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

    public async Task InvokeAsync([NotNull] HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Request.EnableBuffering();
        RequestResponseLogModel log;
        using (var requestStream = recyclableMemoryStreamManager.GetStream())
        {
            await httpContext.Request.Body.CopyToAsync(requestStream);

            log = new()
            {
                UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
                IP = httpContext.Request.GetClientIpAddress(),
                Host = httpContext.Request.Headers.Host.ToString(),
                RequestMethod = httpContext.Request.Method,
                RequestQueryString = httpContext.Request.QueryString.ToString(),
                RoutValues = httpContext.Request.RouteValues.ToArray(),
            };

            log.ServiceName = httpContext.Request.RouteValues["action"] != null
               ? httpContext.Request.RouteValues["action"].ToString()
               : (string)httpContext.Request.Path;


            log.AuditType = log.ServiceName switch
            {
                "PaymentRequest" => Enums.AuditType.Client,
                "TransactionVerify" => Enums.AuditType.Client,
                "TransactionDetail" => Enums.AuditType.Client,
                _ => Enums.AuditType.User
            };

            if (httpContext.User.Claims.Any())
            {
                _ = long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);
                log.CompanyId = companyId;

                _ = long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
                log.ApplicationId = applicationId;

                _ = long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);
                log.UserId = UserId;

                log.ClientId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
            }
            requestStream.Position = 0;
            using StreamReader streamReader = new(requestStream);
            log.RequestBody = httpContext.Request.Path == "/api/" ? "" : await streamReader.ReadToEndAsync();
        }

        httpContext.Request.Body.Position = 0;

        var originalBodyStream = httpContext.Response.Body;
        using (var responseBody = recyclableMemoryStreamManager.GetStream())
        {
            httpContext.Response.Body = responseBody;

            log.StartDateTime = DateTime.Now;
            await next(httpContext);
            log.ResponseTime = DateTime.Now;
            TimeSpan timeDifference = log.EndDateTime - log.StartDateTime;
            log.DurationMs = (long)timeDifference.TotalMilliseconds;

            log.ResponseStatus = httpContext.Response.StatusCode.ToString();

            string text;
            using var reader = new StreamReader(httpContext.Response.Body);
            _ = httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            text = await reader.ReadToEndAsync();
            _ = httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            log.ResponseBody = text;
            await responseBody.CopyToAsync(originalBodyStream);
        }
        log.EndDateTime = DateTime.Now;

        using (LogContext.PushProperty("CallLog", log, true))
        {
            logger.LogInformation("[CallLog] {@CallLog}", log);
        }
    }
}
