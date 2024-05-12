using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveDirectDebitProviderException() : AppException(GlobalResource.PaymentRequestNoActiveDirectDebitProvider)
{
    public override string Code => "1001018";
}