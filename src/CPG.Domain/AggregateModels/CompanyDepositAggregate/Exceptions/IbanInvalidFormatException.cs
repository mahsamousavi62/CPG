using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;

public class IbanInvalidFormatException(string iban) : DomainException(string.Format(Resource.InvalidIbanFormat, iban))
{
    public override string Code => "invalid_iban_format";
    public string IbanPrefix { get; } = iban;
}
