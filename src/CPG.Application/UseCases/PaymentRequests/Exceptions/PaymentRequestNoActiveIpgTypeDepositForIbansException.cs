using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveIpgTypeDepositForIbansException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveIpgTypeDepositForIbans, companyName))
{
    public override string Code => "1001024";
}