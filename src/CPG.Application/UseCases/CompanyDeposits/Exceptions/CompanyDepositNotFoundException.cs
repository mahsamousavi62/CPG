using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositNotFoundException(long companyDepositId) :
    ApplicationException(string.Format(GlobalResource.CompanyDepositNotFound, companyDepositId))
{
    public override string Code => "companyDeposit_not_found";
    public long CompanyDepositId { get; } = companyDepositId;
}
