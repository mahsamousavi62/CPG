using System;
using System.Net;
using System.Threading.Tasks;
using Daryaftyar.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApplicationException = Daryaftyar.Application.UseCases.Exceptions.ApplicationException;

namespace Daryaftyar.Infrastructure.ErrorHandling
{
    public class ErrorHandlingMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

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
                ApplicationException exception => new ExceptionResponse(exception.Code, exception.Message, HttpStatusCode.BadRequest),
                _ => new ExceptionResponse("unexpected_error", ex.Message, HttpStatusCode.InternalServerError)
            };

            var result = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.HttpStatusCode;

            return context.Response.WriteAsync(result);
        }

        public class ExceptionResponse
        {
            public string Code { get; }
            public string Message { get; }
            public HttpStatusCode HttpStatusCode { get; }

            public ExceptionResponse(string code, string message, HttpStatusCode httpStatusCode)
            {
                Code = code;
                Message = message;
                HttpStatusCode = httpStatusCode;
            }
        }
	}
}
