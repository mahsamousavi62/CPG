using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using MediatR.Pipeline;

namespace CPG.Application.MediatorRequestPipelines;

public class PostRequestLogger<TRequest, TResponse>(ILogService logService) : IRequestPostProcessor<TRequest, TResponse>
{
    private readonly ILogService _logService = logService;

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        RequestResponseLogModel log = new ()
        {
            AuditType = Enums.AuditType.Develop,
            ServiceName = requestName,
            ResponseBody = response
        };
        _logService.LogInformation("Request completed:{@log}", log);

        return Task.CompletedTask;
    }
}
