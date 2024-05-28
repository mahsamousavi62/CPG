using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoDefaultDepositException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestInactiveIpgTypes, companyName))
{
    public override string Code => "1001010";
}