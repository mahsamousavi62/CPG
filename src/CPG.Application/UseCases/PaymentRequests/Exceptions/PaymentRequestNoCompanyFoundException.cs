using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoCompanyFoundException() : AppException(GlobalResource.PaymentRequestNoCompanyFound)
{
    public override string Code => "1001002";
}