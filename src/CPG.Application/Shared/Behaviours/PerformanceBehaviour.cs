using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using MediatR;

namespace CPG.Application.Shared.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogService logService,
    ICurrentUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogService _logService = logService;
    private readonly ICurrentUser _user = user;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.UserId;
            var userName = string.Empty;
            if (userId != 0) 
                userName = await User.GetUserName(userId);

            var callLog = CallLogModel.CreateSuccess(
                serviceName: requestName,
                providerName: "MediatR",
                requestUri: requestName,
                requestBody: JsonSerializer.Serialize(request),
                responseBody: $"Long running request completed in {elapsedMilliseconds}ms",
                serviceType: null,
                providerType: Enums.ProviderTypeInLog.Internal,
                auditType: Enums.AuditType.Develop,
                userId: userId == 0 ? null : userId,
                responseStatusCode: 200
            );

            callLog.DurationMs = elapsedMilliseconds;

            _logService.LogWarning(callLog);

        }

        return response;
    }
}
