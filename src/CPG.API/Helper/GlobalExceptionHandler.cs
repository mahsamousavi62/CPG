using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Helper;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var code = string.Empty;

        if(exception is DomainException || exception is AppException)
        {
            code = (exception as dynamic).Code;
        }

        RequestResponseLogModel log = new ()
        {
            AuditType = Enums.AuditType.Develop.ToString(),
            StackTrace = exception.StackTrace,
            ResponseBody = exception.Message,
            IsSuccess = false,
            ErrorCode = code,
        };
        _logger.LogError(exception, "Exception occurred: {log}", log);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
