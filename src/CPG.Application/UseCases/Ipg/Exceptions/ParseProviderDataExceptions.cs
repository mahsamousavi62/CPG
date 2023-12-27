using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exception;
public class ParseCompanyIpgProviderDataException(string message) : ApplicationException(string.Format(GlobalResource.ParseCompanyIpgProviderData,message))
{
    public override string Code => "Parse_CompanyIpg_ProviderData_Exception";
}
