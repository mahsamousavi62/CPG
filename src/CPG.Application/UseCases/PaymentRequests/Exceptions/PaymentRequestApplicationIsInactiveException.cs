using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestApplicationIsInactiveException(string persianName, string englishName) : AppException(string.Format(GlobalResource.PaymentRequestApplicationIsInactive, persianName, englishName))
{
    public override string Code => "1001028";
}