using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestNotMatchIbanAndCompanyException() : ApplicationException
        (string.Format(GlobalResource.PaymentRequestNotMatchIbanAndCompany))
    {
        public override string Code => "PaymentRequest_NotMatchIbanAndCompany";
    }
}
