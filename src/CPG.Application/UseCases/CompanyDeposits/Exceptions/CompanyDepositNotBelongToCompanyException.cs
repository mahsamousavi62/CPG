using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class CompanyDepositNotBelongToCompanyException(long companyDepositId) : AppException(GlobalResource.CompanyDepositNotBelongToCompany)
{
    public override string Code => "companyDeposit_not_belong_to_company";    
}