using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.CompanyIPGs.Exceptions;

public class MorethanOneDefaultDepositFoundException() :
    AppException(string.Format(GlobalResource.MorethanOneDefaultDepositFoundException))
{
    public override string Code => "MorethanOneDefaultDepositFoundException";
    
}