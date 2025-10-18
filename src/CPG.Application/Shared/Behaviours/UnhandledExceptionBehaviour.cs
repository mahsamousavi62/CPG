using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel.Logging;
using MediatR;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.Shared.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogService logService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogService _logService = logService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException exc)
        {
            LogError(request, exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Code, exc.Message)]);
        }
        catch (AppException exc)
        {
            LogError(request, exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Code, exc.Message)]);
        }
        catch (Exception exc)
        {
            LogError(request, exc);
            return (TResponse)typeof(TResponse).GetMethod("Failure").Invoke(null, [new Error(exc.Source, exc.Message)]);
        }
    }

    private void LogError(TRequest request, Exception ex)
    {
        var requestName = typeof(TRequest).Name;

        var callLog = CallLogModel.CreateError(
            serviceName: requestName,
            providerName: "MediatR",
            requestUri: requestName,
            requestBody: JsonSerializer.Serialize(request),
            responseBody: ex.Message,
            exception: ex,
            serviceType: Enums.ServiceType.ErrorHandling,
            providerType: Enums.ProviderTypeInLog.Internal,
            auditType: Enums.AuditType.Develop,
            userId: 1
        );

        _logService.LogError(callLog);
    }
}
