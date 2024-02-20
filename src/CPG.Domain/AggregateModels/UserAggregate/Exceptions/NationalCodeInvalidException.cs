using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.UserAggregate.Exceptions;

public class NationalCodeInvalidException(string nationalCode) : DomainException(Resource.InvalidNationalCode)
{
    public override string Code => "invalid_natioanalCode";
    public string NationalCode { get; } = nationalCode;
}