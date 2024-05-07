using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestAllDepositsAreInactiveException(string methodTypes) : AppException(string.Format(GlobalResource.PaymentRequestAllDepositsAreInactive, methodTypes))
{
    public override string Code => "1001019";
}