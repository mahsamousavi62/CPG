using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeInvalidStatusException() : ApplicationException(string.Format(GlobalResource.PaymentRequestCodeInvalidStatus))
{
    public override string Code => "1007004";
}
