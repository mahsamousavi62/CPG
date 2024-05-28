using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoDepositFoundForIbanException(string methodTypes, string companyName) : AppException(string.Format(GlobalResource.PaymentRequestNoDepositFoundForIban, methodTypes, companyName))
{
    public override string Code => "1001021";
}
