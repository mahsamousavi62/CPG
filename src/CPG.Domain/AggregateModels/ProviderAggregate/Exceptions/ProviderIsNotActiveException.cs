using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Exceptions;

internal class ProviderIsNotActiveException : DomainException
{
    public override string Code => "provider_is_already_not_active";
    public long ProviderId { get; }

    public ProviderIsNotActiveException(long providerId) : base(string.Format(Resource.ProviderIsAlreadyNotActive, providerId))
       => ProviderId = providerId;
}
