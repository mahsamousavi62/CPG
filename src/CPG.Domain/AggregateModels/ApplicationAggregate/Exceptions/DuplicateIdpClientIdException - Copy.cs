using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class IdpClientIdIsNotActiveException(string name) : DomainException(Resource.IdpClientIdIsNotActive)
{
    public override string Code => "IdpClientIdIsNotActive";
}