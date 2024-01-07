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
        var log = new RequestResponseLogModel();
        log.AuditType = Enums.AuditType.Develop.ToString();
        log.StackTrace = exception.StackTrace;
        log.ResponseBody = exception.Message;
        log.IsSuccess = false;
        log.ErrorCode = (exception as dynamic)?.Code;
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
