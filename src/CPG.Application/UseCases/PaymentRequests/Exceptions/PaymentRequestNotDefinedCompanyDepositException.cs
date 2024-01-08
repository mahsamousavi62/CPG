using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions
{
    public class PaymentRequestNotDefinedCompanyDepositException():AppException
        (string.Format(GlobalResource.PaymentRequestNotDefinedCompanyDeposit))
    {
        public override string Code => "PaymentRequest_NotDefinedCompanyDeposit";
    }
}
