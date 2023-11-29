using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;

public class IbanInvalidException(string iban) : DomainException(string.Format(Resource.InvalidIban, iban))
{
    public override string Code => "invalid_iban";
    public string IbanPrefix { get; } = iban;
}
