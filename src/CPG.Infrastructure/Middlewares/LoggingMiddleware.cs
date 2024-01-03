using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.IO;
using CPG.Domain.SharedKernel.Helper.CallLog;
using CPG.Domain.SharedKernel;
using Serilog.Context;

namespace CPG.Infrastructure.Middlewares;

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

    public async Task InvokeAsync([NotNull] HttpContext httpContext)
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

            log.AuditType = Enums.AuditType.User ;
            log.RequestTime = DateTime.Now;
            log.RequestMethod = httpContext.Request.Method;
            log.RequestQueryString = httpContext.Request.QueryString.ToString();

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
