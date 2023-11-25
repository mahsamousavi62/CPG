using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidLogoFormatException:DomainException
{
    public override string Code => "invalid_logo_Format";
    public InvalidLogoFormatException(string message) : base(message)
    {
    }
}
