using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

internal class DuplicateCallbackUrlException(string name) : DomainException(Resource.Duplicate_CallbackUrl)
{
    public override string Code => "duplicate_CallbackUrl";
}