using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoDefaultDirectDebitDepositException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoDefaultDirectDebitDeposit, companyName))
{
    public override string Code => "1001015";
}