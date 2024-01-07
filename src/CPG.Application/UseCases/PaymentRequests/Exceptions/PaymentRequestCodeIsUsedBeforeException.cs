using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCodeIsUsedBeforeException() : ApplicationException(string.Format(GlobalResource.PaymentRequestCodeIsUsedBefore))
{
    public override string Code => "1007003";
}