using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class CompanyIsInactiveException(string companyName) : AppException(string.Format(GlobalResource.CompanyIsInactive, companyName))
{
    public override string Code => "1006005";
}