using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class DuplicateProviderPersianNameException(string persianName) : ApplicationException(string.Format(GlobalResource.DuplicatePersianName, persianName))
{
    public override string Code => "duplicate_provider_persianName";
    public string PersianName { get; } = persianName;
}
