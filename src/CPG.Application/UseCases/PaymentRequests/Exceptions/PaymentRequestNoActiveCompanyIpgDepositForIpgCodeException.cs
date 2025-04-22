using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

internal class PaymentRequestNoActiveCompanyIpgDepositForIpgCodeException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveCompanyIpgDepositForIpgCode, companyName))
{
    public override string Code => "1001030";
}