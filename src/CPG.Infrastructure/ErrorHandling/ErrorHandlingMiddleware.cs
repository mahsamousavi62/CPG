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

            var errorTime = DateTime.Now;

            var callLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = context.TraceIdentifier,
                LogId = $"{Guid.NewGuid()} - ErrorHandling - {ex.GetType().Name}",
                RequestId = context.TraceIdentifier,
                AuditLevel = 3, // Error level
                AuditType = Enums.AuditType.Develop,
                ServiceName = "ErrorHandlingMiddleware",
                ProviderName = "Internal",
                RequestUri = context.Request.Path,
                RequestHeader = context.Request.Headers != null ? System.Text.Json.JsonSerializer.Serialize(context.Request.Headers) : null,
                RequestBody = context.Request.Path,
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = ex.Message,
                ApplicationId = applicationId == 0 ? null : applicationId,
                UserId = userId == 0 ? null : userId,
                Ip = context.Connection.RemoteIpAddress?.ToString(),
                CompanyId = companyId == 0 ? null : companyId,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                Response = ex.Message,
                ErrorCode = ex.GetType().Name,
                IsSucceeded = false,
                StartDateTime = errorTime,
                EndDateTime = errorTime,
                DurationMs = 0,
                StackTrace = ex.StackTrace,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = context.Request.Path,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.ErrorHandling,
                CreationDate = DateTime.Now,
                CreationUserId = userId == 0 ? 1 : userId,
                ErrorType = ex.Message,
                ProviderType = Enums.ProviderTypeInLog.Internal,
                CorrolationId = context.TraceIdentifier
            };
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
