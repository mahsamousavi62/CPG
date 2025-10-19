using HotChocolate;
using CPG.Domain.SharedKernel.Logging;

namespace CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;

public class GraphQLErrorFilter(ILogService logService) : IErrorFilter
{
    private readonly ILogService _logService = logService;

    public IError OnError(IError error)
    {
        var message = $"Error when executing GraphQL query: {error.Exception?.Message ?? error.Message}";
			
        var callLog = CallLogModel.CreateError(
            serviceName: "GraphQL",
            providerName: "GraphQLErrorFilter",
            requestUri: "GraphQL Query",
            requestBody: null,
            responseBody: message,
            exception: error.Exception,
            auditType: Enums.AuditType.Client
        );
        _logService.LogError(callLog);
			
        return error.WithMessage(error.Exception?.Message ?? error.Message);
    }
}