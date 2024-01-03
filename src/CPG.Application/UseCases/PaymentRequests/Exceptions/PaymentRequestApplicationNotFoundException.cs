
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestApplicationNotFoundException() : ApplicationException(string.Format(GlobalResource.PaymentRequestApplicationNotFound))
{
    public override string Code => "1001013";    
}