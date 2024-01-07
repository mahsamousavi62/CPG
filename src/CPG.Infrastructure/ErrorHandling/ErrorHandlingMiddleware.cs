using System;
using System.Net;
using System.Threading.Tasks;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Infrastructure.ErrorHandling;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var log = new RequestResponseLogModel();
            log.AuditType = Enums.AuditType.Develop.ToString();
            log.StackTrace = ex.StackTrace;
            log.ResponseBody = ex.Message;
            log.IsSuccess = false;
            log.ErrorCode = (ex as dynamic)?.Code;

            _logger.LogError("Error details: {@log}", log);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = ex switch
        {
            DomainException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
            ApplicationException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
            _ => new ExceptionResponse("unexpected_error", ex.Message, HttpStatusCode.InternalServerError)
        };

        var result = JsonConvert.SerializeObject(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.HttpStatusCode;

        return context.Response.WriteAsync(result);
    }

    public class ExceptionResponse(string code, string message, HttpStatusCode httpStatusCode)
    {
        public string Code { get; } = code;
        public string Message { get; } = message;
        public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;
    }
}
