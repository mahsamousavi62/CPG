using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoCompanyIpgDepositForIpgCodeException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoCompanyIpgDepositForIpgCode, companyName))
{
    public override string Code => "1001029";
}