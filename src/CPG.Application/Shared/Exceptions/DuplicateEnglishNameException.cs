using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.Shared.Exceptions;

public class DuplicateEnglishNameException(string englishName) : ApplicationException(string.Format(GlobalResource.DuplicateEnglishName, englishName))
{
    public override string Code => "duplicate_englishName";
    public string EnglishName { get; } = englishName;
}
