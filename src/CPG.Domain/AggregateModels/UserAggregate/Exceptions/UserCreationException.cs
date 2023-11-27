using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.UserAggregate.Exceptions;

public class UserCreationException(string message) : DomainException(message)
{
    public override string Code => "cannot_create_user";
}

