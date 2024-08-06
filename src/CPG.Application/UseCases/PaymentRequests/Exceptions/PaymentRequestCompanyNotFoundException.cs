using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCompanyNotFoundException() : AppException(GlobalResource.CompanyNotFound)
{
    public override string Code => "company_not_found";
}
