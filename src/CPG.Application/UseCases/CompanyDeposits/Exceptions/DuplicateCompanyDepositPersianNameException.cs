using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class DuplicatCompanyDepositPersianNameException(string persianName) 
    : ApplicationException(string.Format(GlobalResource.DuplicatePersianName, persianName))
{
    public override string Code => "duplicate_companydeposit_persianName";
    public string PersianName { get; } = persianName;
}
