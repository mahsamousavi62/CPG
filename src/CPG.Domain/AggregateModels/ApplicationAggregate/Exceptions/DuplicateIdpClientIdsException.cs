using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class DuplicateIdpClientIdsException(string name) : DomainException(string.Format(Resource.Duplicate_IdpClientIds, name))
{
    public override string Code => "duplicate_IdpClientIds";
}