using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestIbanCompanyInActiveException() : AppException(
        string.Format(GlobalResource.PaymentRequestIbanCompanyInActive))
    {
        public override string Code => "PaymentRequest_IbanCompanyInActive";
    }

}
