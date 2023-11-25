using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class InvalidCredentialsException(string message) : DomainException(message)
{
    public override string Code => "invalid_credentials";
}