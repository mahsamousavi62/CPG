using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Helper;

internal sealed class GlobalExceptionHandler(ILogService logService) : IExceptionHandler
{
    private readonly ILogService _logService = logService;

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

        var callLog = CallLogModel.CreateError(
            serviceName: "GlobalExceptionHandler",
            providerName: "API",
            requestUri: httpContext.Request.Path.ToString(),
            requestBody: null,
            responseBody: exception.Message,
            exception: exception,
            auditType: Enums.AuditType.Develop
        );
        callLog.ErrorCode = code;
        callLog.StackTrace = exception.StackTrace;

        _logService.LogError(callLog);

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
