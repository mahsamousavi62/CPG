using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInactiveDepositsException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestInactiveDeposits, companyName))
{
    public override string Code => "1001009";
}