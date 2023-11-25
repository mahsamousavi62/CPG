using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidPathLogoException(string message) : DomainException(message)
{
    public override string Code => "invalid_path_logo";
}
