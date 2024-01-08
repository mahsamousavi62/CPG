using System;
using System.Net;
using System.Threading.Tasks;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            _logger.LogError("Error details: {@ex}", ex);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = ex switch
        {
            DomainException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
            AppException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
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
