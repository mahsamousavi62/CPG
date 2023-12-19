using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeIsExpiredException() : ApplicationException(GlobalResource.PaymentRequestCodeIsExpired)
{
    public override string Code => "1006002";
}