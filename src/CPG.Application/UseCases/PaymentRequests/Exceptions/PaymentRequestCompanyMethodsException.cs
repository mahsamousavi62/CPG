using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCompanyMethodsException(string methods, string companyName) : AppException(string.Format(GlobalResource.PaymentRequestCompanyMethods, methods, companyName))
{
    public override string Code => "1001008";
}