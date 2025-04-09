using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestCompanyCodeRequiredException() : AppException(GlobalResource.PaymentCompanyCodeIsRequired)
{
    public override string Code => "1001037";
}