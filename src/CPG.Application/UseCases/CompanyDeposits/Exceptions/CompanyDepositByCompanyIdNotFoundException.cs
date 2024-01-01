
using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositByCompanyIdNotFoundException(long companyId) :
    ApplicationException(string.Format(GlobalResource.CompanyDepositByCompanyIdNotFound, companyId))
{
    public override string Code => "companyDeposit_by_companyId_not_found";
    public long CompanyId { get; } = companyId;
}
