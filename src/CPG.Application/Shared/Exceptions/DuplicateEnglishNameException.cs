using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.Shared.Exceptions;

public class DuplicateEnglishNameException(string englishName)
    : ApplicationException(string.Format(GlobalResource.DuplicateEnglishName, englishName))
{
    public override string Code => "duplicate_englishName";
    public string EnglishName { get; } = englishName;
}
