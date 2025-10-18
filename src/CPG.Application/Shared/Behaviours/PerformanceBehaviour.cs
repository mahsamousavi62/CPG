using System.Diagnostics;
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

            RequestResponseLogModel log = new()
            {
                RequestMethod = requestName,
                UserId = userId,
                DurationMs = elapsedMilliseconds,
                AuditType = Enums.AuditType.Develop
            };

            _logService.LogWarning("Long Running Request {@log}", log);

        }

        return response;
    }
}
