using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class EmptyPersianNameException(string message) : DomainException(message)
{
    public override string Code => "empt♂y_persianName";
}
