using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeInvalidStatusException() : AppException(string.Format(GlobalResource.PaymentRequestCodeInvalidStatus))
{
    public override string Code => "1007004";
}
