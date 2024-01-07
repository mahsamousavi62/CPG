using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CPG.Application.MediatorRequestPipelines;

public class PostRequestLogger<TRequest, TResponse>(ILogger<TRequest> logger) : IRequestPostProcessor<TRequest, TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var log = new RequestResponseLogModel();
        log.AuditType = Enums.AuditType.Develop.ToString();
        log.ServiceName = requestName;
        log.ResponseBody = response;    
        _logger.LogInformation("Request completed:{@log}", log);

        return Task.CompletedTask;
    }
}
