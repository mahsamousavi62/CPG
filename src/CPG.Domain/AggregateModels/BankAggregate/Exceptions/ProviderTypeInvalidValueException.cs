using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class ProviderTypeInvalidValueException(int providerType) : DomainException(string.Format(Resource.InvalidProviderType, providerType))
    {
    public override string Code => "invalid_providerType_value";
    public int ProviderType { get; } = providerType;
}