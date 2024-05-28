using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoDepositPaymentMethodException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoDepositPaymentMethod, companyName))
{
    public override string Code => "1001005";
}