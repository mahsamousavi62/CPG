using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class CompanyCreationException(string message) : DomainException(message)
{
    public override string Code => "cannot_create_company";
}
