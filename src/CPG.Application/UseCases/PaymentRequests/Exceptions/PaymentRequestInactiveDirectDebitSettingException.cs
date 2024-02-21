using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestInactiveDirectDebitSettingException() : AppException(string.Format(GlobalResource.UnexpectedError))
{
    public override string Code => "1003014";
}