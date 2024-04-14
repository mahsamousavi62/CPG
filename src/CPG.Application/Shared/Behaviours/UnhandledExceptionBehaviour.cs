using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel.Logging;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.Shared.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException exc)
        {
            LogError(exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Code, exc.Message)]);
        }
        catch (AppException exc)
        {
            LogError(exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Code, exc.Message)]);
        }
        catch (Exception exc)
        {
            LogError(exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Source, exc.Message)]);
        }
    }

    private void LogError(Exception ex)
    {
        var requestName = typeof(TRequest).Name;

        bool hasCode = ex.HasProperty("code");

        RequestResponseLogModel log = new()
        {
            AuditType = Enums.AuditType.Develop,
            ServiceName = requestName,
            StackTrace = ex.StackTrace,
            ResponseBody = ex.Message,
            IsSuccess = false,
            ErrorCode = hasCode ? (ex as dynamic)?.Code : string.Empty,
        };

        _logger.LogError(ex, "Request: Unhandled Exception for Request {Name} {@log}", requestName, log);
    }
}
