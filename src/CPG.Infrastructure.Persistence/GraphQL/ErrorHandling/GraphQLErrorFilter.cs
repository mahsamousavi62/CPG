using HotChocolate;
using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Persistence.GraphQL.ErrorHandling;

public class GraphQLErrorFilter(ILogger<GraphQLErrorFilter> logger) : IErrorFilter
{
    private readonly ILogger<GraphQLErrorFilter> _logger = logger;

    public IError OnError(IError error)
    {
        var message = $"Error when executing GraphQL query: {error.Exception?.Message ?? error.Message}";
			
        _logger.LogError(message);
			
        return error.WithMessage(error.Exception?.Message ?? error.Message);
    }
}