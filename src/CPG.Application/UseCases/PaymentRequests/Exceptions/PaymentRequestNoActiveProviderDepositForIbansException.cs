using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveProviderDepositForIbansException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveProviderDepositForIbans, companyName))
{
    public override string Code => "1001028";
}