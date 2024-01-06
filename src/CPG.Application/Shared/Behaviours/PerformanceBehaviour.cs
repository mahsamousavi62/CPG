using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CPG.Application.Shared.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;
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

            var log = new RequestResponseLogModel();
            log.RequestMethod = requestName;
            log.UserId= userId;
            log.ElapsedMilliseconds = elapsedMilliseconds;
            log.AuditType = Enums.AuditType.Develop.ToString();
         
            //_logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
            //    requestName, elapsedMilliseconds, userId, userName, request);
            
            _logger.LogWarning("LogDetail", log);

        }

        return response;
    }
}
