using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CPG.Application.MediatorRequestPipelines;

public class PreRequestLogger<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        RequestResponseLogModel log = new()
        {
            AuditType = Enums.AuditType.Develop.ToString(),
            ServiceName = requestName
        };

        _logger.LogInformation("Request started: {log}", log);

        return Task.CompletedTask;
    }
}
