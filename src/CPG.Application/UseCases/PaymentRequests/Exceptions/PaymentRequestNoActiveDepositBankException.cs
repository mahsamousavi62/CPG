using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveDepositBankException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveDepositBank, companyName))
{
    public override string Code => "1001004";
}
