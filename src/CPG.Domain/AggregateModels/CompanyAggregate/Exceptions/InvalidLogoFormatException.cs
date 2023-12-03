using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidLogoFormatException(string message)
        : DomainException(string.Format(Resource.Invalid_Logo_Format, message))
{
    public override string Code => "invalid_logo_Format";
}
