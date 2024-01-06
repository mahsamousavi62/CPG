using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestBankInactiveException() : ApplicationException(string.Format(GlobalResource.PaymentRequestBankInactive))
{
    public override string Code => "1001007";
}
