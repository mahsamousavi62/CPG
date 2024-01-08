using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositNotFoundException(long companyDepositId) :
    AppException(string.Format(GlobalResource.CompanyDepositNotFound, companyDepositId))
{
    public override string Code => "companyDeposit_not_found";
    public long CompanyDepositId { get; } = companyDepositId;
}
