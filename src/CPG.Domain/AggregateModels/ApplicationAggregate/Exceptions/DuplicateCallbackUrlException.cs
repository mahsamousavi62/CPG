using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class DuplicateCallbackUrlException(string name) : DomainException(Resource.Duplicate_CallbackUrl)
{
    public override string Code => "duplicate_CallbackUrl";
}