using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Exceptions;

public class ProviderIsActiveException : DomainException
{
    public override string Code => "provider_is_already_active";
    public long ProviderId { get; }

    public ProviderIsActiveException(long providerId) : base(string.Format(Resource.ProviderIsAlreadyActive, providerId))
       => ProviderId = providerId;
}
