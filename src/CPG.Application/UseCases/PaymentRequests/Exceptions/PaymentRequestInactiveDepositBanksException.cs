using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

internal class PaymentRequestInactiveDepositBanksException(string paymentMethods, string companyName) : AppException(string.Format(GlobalResource.PaymentRequestInactiveDepositBanks, paymentMethods, companyName))
{
    public override string Code => "1001006";
}
