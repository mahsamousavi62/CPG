using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidPersianNameException(string message) : DomainException(message)
{
    public override string Code => "invalid_persianName";
}
