using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class DuplicatCompanyDepositPersianNameException(string persianName) 
    : AppException(string.Format(GlobalResource.DuplicatePersianName, persianName))
{
    public override string Code => "duplicate_companydeposit_persianName";
    public string PersianName { get; } = persianName;
}
