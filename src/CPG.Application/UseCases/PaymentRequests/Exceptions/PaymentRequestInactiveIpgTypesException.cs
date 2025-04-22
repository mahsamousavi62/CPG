using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInactiveIpgTypesException() : AppException(GlobalResource.PaymentRequestInactiveIpgTypes)
{
    public override string Code => "1001013";
}