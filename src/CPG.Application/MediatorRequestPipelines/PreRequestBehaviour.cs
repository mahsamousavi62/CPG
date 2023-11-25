using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CPG.Application.MediatorRequestPipelines;

	public class PreRequestLogger<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Request started: {requestName}", requestName);

        return Task.CompletedTask;
    }
}
