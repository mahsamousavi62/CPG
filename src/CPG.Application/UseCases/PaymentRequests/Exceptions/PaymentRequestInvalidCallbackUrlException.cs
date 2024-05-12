using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInvalidCallbackUrlException(string callbackUrl) : AppException(string.Format(GlobalResource.PaymentRequestInvalidCallbackUrl, callbackUrl))
{
    public override string Code => "1001035";
}