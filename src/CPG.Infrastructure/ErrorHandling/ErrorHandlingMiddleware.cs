using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.ErrorHandling;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogService logService)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogService _logService = logService;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _ = long.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long userId);
            _ = long.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: "ErrorHandlingMiddleware",
                providerName: "Internal",
                requestUri: context.Request.Path,
                requestBody: context.Request.Path,
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.ErrorHandling,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                correlationId: context.TraceIdentifier,
                userId: userId,
                applicationId: applicationId,
                companyId: companyId,
                ip: context.Connection.RemoteIpAddress?.ToString(),
                userAgent: context.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogError(callLog);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = ex switch
        {
            DomainException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
            AppException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
            _ => new ExceptionResponse("unexpected_error", ex.Message, HttpStatusCode.InternalServerError)
        };

        var result = JsonConvert.SerializeObject(response);
        using MemoryStream responseBody = new();

        byte[] messageBytes = Encoding.UTF8.GetBytes(result);
        responseBody.Write(messageBytes, 0, messageBytes.Length);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.HttpStatusCode;
        context.Response.Body = new MemoryStream(messageBytes);
        var writer = context.Response.BodyWriter;
        await writer.WriteAsync(messageBytes);
        await writer.CompleteAsync();
        
    }

    public class ExceptionResponse(string code, string message, HttpStatusCode httpStatusCode)
    {
        public string Code { get; } = code;
        public string Message { get; } = message;
        public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;
    }
}
