using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoActiveIpgTypeException(string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoActiveIpgType, companyName))
{
    public override string Code => "1001016";
}