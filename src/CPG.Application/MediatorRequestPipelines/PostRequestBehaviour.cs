using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CPG.Application.MediatorRequestPipelines;

public class PostRequestLogger<TRequest, TResponse>(ILogger<TRequest> logger) : IRequestPostProcessor<TRequest, TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Request completed: {requestName} with response: {@response}", requestName, response);

        return Task.CompletedTask;
    }
}
