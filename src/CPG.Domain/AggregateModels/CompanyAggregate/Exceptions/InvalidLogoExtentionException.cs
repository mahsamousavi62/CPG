using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class InvalidLogoExtentionException(string message) : DomainException(message)
{
    public override string Code => "invalid_logo_extention";
}
