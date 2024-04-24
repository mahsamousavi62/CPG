using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Companies.Exceptions;

public class DuplicateCodeException(short code) : AppException(string.Format(GlobalResource.DuplicateCompanyCode, code))
{
    public override string Code => "duplicate_companyCode";
}