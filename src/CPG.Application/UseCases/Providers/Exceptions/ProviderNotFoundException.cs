using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class ProviderNotFoundException(long providerId) : ApplicationException($"Provider with ID {providerId} has not been found.")
{
    public override string Code => "provider_not_found";
    public long ProviderId { get; } = providerId;
}