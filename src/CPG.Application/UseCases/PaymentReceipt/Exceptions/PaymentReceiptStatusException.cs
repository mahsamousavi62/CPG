using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentReceipt.Exceptions;
public class VerifyPaymentReceiptStatusException() : AppException(string.Format(GlobalResource.VerifyInvalidStatus))
{
    public override string Code => "1015002";
}
