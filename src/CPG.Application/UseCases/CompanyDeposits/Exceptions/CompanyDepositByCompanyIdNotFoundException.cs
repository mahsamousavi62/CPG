
using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositByCompanyIdNotFoundException(long companyId) :
    AppException(string.Format(GlobalResource.CompanyDepositByCompanyIdNotFound, companyId))
{
    public override string Code => "companyDeposit_by_companyId_not_found";
    public long CompanyId { get; } = companyId;
}
