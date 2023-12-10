using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.CompanyIPGs.Exceptions;

public class CompanyIPGNotFoundException(long companyIPGId) :
    ApplicationException(string.Format(GlobalResource.CompanyIPGNotFound, companyIPGId))
{
    public override string Code => "company_ipg_not_found";
    public long CompanyIPGId { get; } = companyIPGId;
}