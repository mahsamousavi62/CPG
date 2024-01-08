using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class ProviderNotFoundException(long providerId) : AppException(string.Format(GlobalResource.ProviderNotFound, providerId))
{
    public override string Code => "provider_not_found";
    public long ProviderId { get; } = providerId;
}