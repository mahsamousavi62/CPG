using System.Text.Json;
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
        var callLog = CallLogModel.CreateSuccess(
            serviceName: requestName,
            providerName: "MediatR",
            requestUri: requestName,
            requestBody: JsonSerializer.Serialize(request),
            responseBody: "Request started",
            serviceType: null,
            providerType: Enums.ProviderTypeInLog.Internal,
            auditType: Enums.AuditType.Develop,
            userId: 1,
            responseStatusCode: 200
        );

        _logService.LogInformation(callLog);

        return Task.CompletedTask;
    }
}
