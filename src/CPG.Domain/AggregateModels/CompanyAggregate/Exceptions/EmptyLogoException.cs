using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class EmptyLogoException(string message) : DomainException(message)
{
    public override string Code => "empty_logo";
}
