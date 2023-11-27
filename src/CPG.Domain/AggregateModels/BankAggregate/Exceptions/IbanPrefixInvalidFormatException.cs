using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class IbanPrefixInvalidFormatException(string ibanPrefix) : DomainException(string.Format(Resource.InvalidIbanPrefix, ibanPrefix))
{
    public override string Code => "invalid_ibanPrefix_format";
    public string IbanPrefix { get; } = ibanPrefix;
}
