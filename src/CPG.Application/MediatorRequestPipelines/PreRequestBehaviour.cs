using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;
using MediatR.Pipeline;

namespace CPG.Application.MediatorRequestPipelines;

public class PreRequestLogger<TRequest>(ILogService logService) : IRequestPreProcessor<TRequest>
{
    private readonly ILogService _logService = logService;

    public Task Process(TRequest request, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        RequestResponseLogModel log = new()
        {
            AuditType = Enums.AuditType.Develop,
            ServiceName = requestName
        };

        _logService.LogInformation("Request started: {log}", log);

        return Task.CompletedTask;
    }
}
