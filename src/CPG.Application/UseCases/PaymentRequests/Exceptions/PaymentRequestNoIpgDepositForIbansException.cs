using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoIpgDepositForIbansException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoIpgDepositForIbans, companyName))
{
    public override string Code => "1001022";
}