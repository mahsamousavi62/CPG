using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class ProviderNotFoundException(long providerId) : ApplicationException(string.Format(GlobalResource.ProviderNotFound, providerId))
{
    public override string Code => "provider_not_found";
    public long ProviderId { get; } = providerId;
}