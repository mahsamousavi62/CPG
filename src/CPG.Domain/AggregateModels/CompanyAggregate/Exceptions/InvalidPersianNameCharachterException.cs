using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidPersianNameCharachterException(string name) :
DomainException(string.Format(Resource.Invalid_PersianNameCharachterLimit, name))
{
    public override string Code => "Invalid_PersianNameCharachterLimit";
}

