using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestNotFoundException(string paymentRequestCode) : AppException(string.Format(GlobalResource.PaymentRequestNotFound, paymentRequestCode))
    {
        public override string Code => "PaymentRequest_NotFound";

    }
}
