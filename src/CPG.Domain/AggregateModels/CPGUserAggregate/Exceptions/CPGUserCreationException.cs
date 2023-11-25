using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class CPGUserCreationException(string message) : DomainException(message)
{
    public override string Code => "cannot_create_CPG_user";
}
