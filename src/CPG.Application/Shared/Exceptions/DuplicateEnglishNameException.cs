using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.Shared.Exceptions;

public class DuplicateEnglishNameException(string englishName)
    : AppException(string.Format(GlobalResource.DuplicateEnglishName, englishName))
{
    public override string Code => "duplicate_englishName";
    public string EnglishName { get; } = englishName;
}
