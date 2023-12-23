using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeIsFinalizedException() : ApplicationException(GlobalResource.PaymentRequestCodeIsFinalized)
{
    public override string Code => "1006004";
}