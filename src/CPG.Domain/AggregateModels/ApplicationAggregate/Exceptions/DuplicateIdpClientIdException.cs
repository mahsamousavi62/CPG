using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class DuplicateIdpClientIdException(string name) : DomainException(Resource.Duplicate_IdpClientId)
{
    public override string Code => "duplicate_IdpClientId";
}