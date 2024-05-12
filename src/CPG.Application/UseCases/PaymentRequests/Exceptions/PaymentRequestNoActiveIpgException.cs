using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveIpgException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveIpg, companyName))
{
    public override string Code => "1001015";
}