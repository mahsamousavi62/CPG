using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.CompanyDeposits.Exceptions;

public class MethodTypeNotAllowedException(string message) : AppException(GlobalResource.MethodTypeNotAllowed)
{
    public override string Code => "methodType_not_allowed";    
}