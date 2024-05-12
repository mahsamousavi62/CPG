using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInvalidNationalCodeException() : AppException(GlobalResource.PaymentRequestInvalidNationalCode)
{
    public override string Code => "1001033";
}