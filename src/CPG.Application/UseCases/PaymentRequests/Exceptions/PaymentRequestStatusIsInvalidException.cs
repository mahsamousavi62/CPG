using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestStatusIsInvalidException() : ApplicationException(GlobalResource.PaymentRequestStatusIsInvalid)
{
    public override string Code => "1006005";
}