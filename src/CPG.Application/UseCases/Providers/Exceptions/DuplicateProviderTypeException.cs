using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class DuplicateProviderTypeException(Enums.ProviderType providerType) 
    : AppException(string.Format(GlobalResource.DuplicateProviderType, providerType.ToString()))
{
    public override string Code => "duplicate_provider_providerType";
}
