using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestAllDepositsNotSupportMethodException(string methodTypes) : AppException(string.Format(GlobalResource.PaymentRequestAllDepositsNotSupportMethod, methodTypes))
{
    public override string Code => "1001021";
}