using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.Shared.Exceptions;

public class DuplicatePersianNameException(string persianName)
    : AppException(string.Format(GlobalResource.DuplicatePersianName, persianName))
{
    public override string Code => "duplicate_persianName";
    public string PersianName { get; } = persianName;
}
