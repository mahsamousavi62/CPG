using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestAllDepositBanksAreInactiveException(string methodTypes) : AppException(string.Format(GlobalResource.PaymentRequestAllDepositBanksAreInactive, methodTypes))
{
    public override string Code => "1001020";
}