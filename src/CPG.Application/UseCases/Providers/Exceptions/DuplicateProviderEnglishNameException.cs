using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Providers.Exceptions;

public class DuplicateProviderEnglishNameException(string englishName) : ApplicationException(string.Format(GlobalResource.DuplicateEnglishName,englishName))
{
    public override string Code => "duplicate_provider_englishName";
    public string EnglishName { get; } = englishName;
}
