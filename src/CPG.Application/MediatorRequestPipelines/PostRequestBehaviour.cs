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

        var callLog = CallLogModel.CreateSuccess(
            serviceName: requestName,
            providerName: "MediatR",
            requestUri: requestName,
            requestBody: System.Text.Json.JsonSerializer.Serialize(request),
            responseBody: System.Text.Json.JsonSerializer.Serialize(response),
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
