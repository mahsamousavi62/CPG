using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeIsUsedException() : AppException(GlobalResource.PaymentRequestCodeIsUsed)
{
    public override string Code => "1006003";
}