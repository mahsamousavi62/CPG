using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveIpgDepositForIbansException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveIpgDepositForIbans, companyName))
{
    public override string Code => "1001026";
}