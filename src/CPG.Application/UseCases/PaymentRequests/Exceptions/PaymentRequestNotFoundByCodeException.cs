
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNotFoundByCodeException() : AppException(string.Format(GlobalResource.PaymentRequestNotFoundByCode))
{
    public override string Code => "1007001";
}
