using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNationalCodeRequiredException() : AppException(GlobalResource.IsAnonymousNationalCodeRequired)
{
    public override string Code => "1001037";
}