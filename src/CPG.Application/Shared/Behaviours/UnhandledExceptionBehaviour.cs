using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.Shared.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var log = new RequestResponseLogModel();
            log.AuditType = Enums.AuditType.Develop.ToString();
            log.ServiceName = requestName;
            log.StackTrace = ex.StackTrace;
            log.ResponseBody = ex.Message;
            log.IsSuccess = false;
            log.ErrorCode = (ex as dynamic)?.Code;

            _logger.LogError(ex, "Request: Unhandled Exception for Request {Name} {@log}", requestName, log);

            throw;
        }
    }
}
