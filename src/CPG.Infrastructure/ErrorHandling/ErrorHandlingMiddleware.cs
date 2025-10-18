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
            var callLog = new CallLogModel
            {
                RequestBody = context.Request.Path,
                ResponseBody = ex.Message,
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = context.Request.Path,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.ErrorHandling,
                CreationDate = DateTime.Now,
                CreationUserId = 1,
                ErrorCode = ex.GetType().Name,
                ErrorType = ex.Message,
                ProviderType = Enums.ProviderTypeInLog.Internal,
                AuditType = Enums.AuditType.Develop,
                StackTrace = ex.StackTrace
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
