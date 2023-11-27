using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class ProviderNotFoundException(int providerId) : ApplicationException($"Provider with ID {providerId} has not been found.")
{
    public override string Code => "provider_not_found";
    public int ProviderId { get; } = providerId;
}