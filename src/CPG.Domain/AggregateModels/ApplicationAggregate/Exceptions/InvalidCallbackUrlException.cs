using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class InvalidCallbackUrlException(string name) : DomainException(Resource.Invalid_CallbackUrl)
{
    public override string Code => "invalid_CallbackUrl";
}