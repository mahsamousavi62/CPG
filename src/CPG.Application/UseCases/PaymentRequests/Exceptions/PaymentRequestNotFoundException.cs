using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestNotFoundException(long paymentRequestId) : ApplicationException(string.Format(GlobalResource.PaymentRequestNotFound,paymentRequestId))
    {
        public override string Code => "PaymentRequest_NotFound";

    }
}
