using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.IO;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using Serilog.Context;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CPG.Infrastructure.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;
    private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;

    public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        this.next = next;
        recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        logger = loggerFactory.CreateLogger<LoggingMiddleware>();
    }


    public async Task InvokeAsync([NotNull] HttpContext httpContext, IAuthenticationService authenticationService)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (!httpContext.Request.Path.Value?.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            await next(httpContext);
            return;
        }

        httpContext.Request.EnableBuffering();
        var log = new RequestResponseLogModel();

        using (var requestStream = recyclableMemoryStreamManager.GetStream())
        {
            await httpContext.Request.Body.CopyToAsync(requestStream);
            log.UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            log.IP = httpContext.Request.GetClientIpAddress();
            log.Host = httpContext.Request.Headers["Host"].ToString();
            log.ServiceName = httpContext.Request.Path;
            log.AuditType = httpContext.User.FindFirst("AuditType")?.Value ?? Enums.AuditType.User.ToString();
            log.RequestTime = DateTime.Now;
            log.RequestMethod = httpContext.Request.Method;
            log.RequestQueryString = httpContext.Request.QueryString.ToString();
            
            if (httpContext.User.Claims.Count() != 0)
            {
                long companyId, applicationId, UserId;
                long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out companyId);
                log.CompanyId = companyId;

                long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out applicationId);
                log.ApplicationId = applicationId;

                _ = long.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out UserId);
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
            await next(httpContext);

            log.ResponseStatus = httpContext.Response.StatusCode.ToString();
            log.ResponseTime = DateTime.Now;

            string text;
            using var reader = new StreamReader(httpContext.Response.Body);
            _ = httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            text = await reader.ReadToEndAsync();
            _ = httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            log.ResponseBody = text;

            await responseBody.CopyToAsync(originalBodyStream);
        }

        using (LogContext.PushProperty("CallLog", log, true))
        {
            logger.LogInformation("[CallLog] {@CallLog}", log);
        }
    }
}
