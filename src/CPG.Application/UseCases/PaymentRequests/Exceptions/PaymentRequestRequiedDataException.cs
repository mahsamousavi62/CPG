using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestRequiredDataException() :
        ApplicationException(string.Format(GlobalResource.PaymentRequestRequiredIbanOrCompany))
    {
        public override string Code => "paymentRequest_required_IbanOrCompany";
    }


}
