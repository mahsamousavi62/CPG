using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Companies.Exceptions;

public class CompanyNotFoundException(long companyId) :
    AppException(string.Format(GlobalResource.CompanyNotFound, companyId))
{
    public override string Code => "company_not_found";
    public long CompanyId { get; } = companyId;
}
