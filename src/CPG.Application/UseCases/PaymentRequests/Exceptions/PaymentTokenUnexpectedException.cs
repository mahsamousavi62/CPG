using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentTokenUnexpectedException() : AppException(string.Format(GlobalResource.PaymentTokenUnexpectedError))
{
    public override string Code => "1003000";
}
