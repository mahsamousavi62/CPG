using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCompanyHasNoReceiptMethodException() : AppException(GlobalResource.UnexpectedError)
{
    public override string Code => "1003021";
}
