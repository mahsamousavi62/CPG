using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Logging;

public class LoggingMiddleware(RequestDelegate next, ILogService logService)
{
    private readonly RequestDelegate next = next;
    private readonly ILogService _logService = logService;

    public async Task InvokeAsync([NotNull] HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        try
        {
            httpContext.Request.EnableBuffering();
            RequestResponseLogModel log;

            using (MemoryStream requestStream = new())
            {
                await httpContext.Request.Body.CopyToAsync(requestStream);

                log = new()
                {
                    UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
                    IP = httpContext.Request.GetClientIpAddress(),
                    Host = httpContext.Request.Headers.Host.ToString(),
                    RequestMethod = httpContext.Request.Method,
                    RequestQueryString = httpContext.Request.QueryString.ToString(),
                    RoutValues = [.. httpContext.Request.RouteValues],
                    ServiceName = httpContext.Request.RouteValues["action"] != null
                   ? httpContext.Request.RouteValues["action"].ToString()
                   : (string)httpContext.Request.Path
                };


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

                    log.MobilePhone = httpContext.User.Claims.FirstOrDefault(c => c.Type == "MobilePhone")?.Value;
                }
                requestStream.Position = 0;
                using StreamReader streamReader = new(requestStream);
                log.RequestBody = httpContext.Request.Path == "/api/" ? "" : await streamReader.ReadToEndAsync();
            }

            httpContext.Request.Body.Position = 0;

            var originalBodyStream = httpContext.Response.Body;

            using (MemoryStream responseBody = new())
            {
                httpContext.Response.Body = responseBody;

                log.StartDateTime = DateTime.Now;

                await next(httpContext);

                log.EndDateTime = DateTime.Now;

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

            var callLog = CallLogModel.CreateSuccess(
                serviceName: log.ServiceName ?? "Unknown",
                providerName: "Logging",
                requestUri: httpContext.Request.Path.ToString(),
                requestBody: log.RequestBody as string ?? string.Empty,
                responseBody: log.ResponseBody as string ?? string.Empty,
                auditType: log.AuditType,
                userId: log.UserId,
                applicationId: log.ApplicationId,
                companyId: log.CompanyId,
                ip: log.IP,
                userAgent: log.UserAgent,
                responseStatusCode: httpContext.Response.StatusCode,
                requestHeader: System.Text.Json.JsonSerializer.Serialize(httpContext.Request.Headers),
                responseHeader: System.Text.Json.JsonSerializer.Serialize(httpContext.Response.Headers)
            );
            callLog.StartDateTime = log.StartDateTime;
            callLog.EndDateTime = log.EndDateTime;
            callLog.DurationMs = log.DurationMs;

            using (LogContext.PushProperty("CallLog", callLog, true))
            {
                _logService.LogInformation(callLog);
            }
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(LoggingMiddleware),
                providerName: "Logging",
                requestUri: httpContext.Request.Path.ToString(),
                requestBody: null,
                responseBody: exc.Message,
                exception: exc,
                auditType: Enums.AuditType.Develop,
                requestHeader: System.Text.Json.JsonSerializer.Serialize(httpContext.Request.Headers),
                responseHeader: httpContext.Response?.Headers != null ? System.Text.Json.JsonSerializer.Serialize(httpContext.Response.Headers) : null
            );
            _logService.LogError(callLog);
            throw;
        }
    }
}
