using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveProviderException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveProvider, companyName))
{
    public override string Code => "1001012";
}