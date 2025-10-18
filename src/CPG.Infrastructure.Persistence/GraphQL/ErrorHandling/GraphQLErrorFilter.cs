using HotChocolate;
using CPG.Domain.SharedKernel.Logging;

namespace CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;

public class GraphQLErrorFilter(ILogService logService) : IErrorFilter
{
    private readonly ILogService _logService = logService;

    public IError OnError(IError error)
    {
        var message = $"Error when executing GraphQL query: {error.Exception?.Message ?? error.Message}";
			
        _logService.LogError(message);
			
        return error.WithMessage(error.Exception?.Message ?? error.Message);
    }
}