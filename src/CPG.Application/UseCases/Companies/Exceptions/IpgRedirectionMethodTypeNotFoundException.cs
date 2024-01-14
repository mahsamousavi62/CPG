using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Companies.Exceptions;

public class IpgRedirectionMethodTypeNotFoundException() : AppException(string.Format(GlobalResource.IpgRedirectionMethodTypeNotFound))
{
    public override string Code => "ipgRedirectionMethodType_not_found";    
}
