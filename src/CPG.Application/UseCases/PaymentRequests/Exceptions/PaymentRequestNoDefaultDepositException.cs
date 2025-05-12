using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoDefaultDepositException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoDefaultDeposit, companyName))
{
    public override string Code => "1001014";
}