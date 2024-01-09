using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestBankInactiveException() : AppException(string.Format(GlobalResource.PaymentRequestBankInactive))
{
    public override string Code => "1001007";
}
