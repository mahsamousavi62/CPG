
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class ApplicationNotFoundException() : DomainException(Resource.ApplicationNotFound)
{
    public override string Code => "1018001";
}
