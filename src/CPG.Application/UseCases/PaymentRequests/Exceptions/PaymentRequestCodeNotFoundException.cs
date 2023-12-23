using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeNotFoundException() : ApplicationException(GlobalResource.PaymentRequestCodeNotFound)
{
    public override string Code => "1006001";
}