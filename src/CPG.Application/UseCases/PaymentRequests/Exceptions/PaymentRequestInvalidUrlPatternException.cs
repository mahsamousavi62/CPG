using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInvalidUrlPatternException() : AppException(GlobalResource.PaymentRequestInvalidUrlPattern)
{
    public override string Code => "1001034";
}