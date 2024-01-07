using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeExpiredException() : ApplicationException(string.Format(GlobalResource.PaymentRequestCodeExpired))
{
    public override string Code => "1007002";
}