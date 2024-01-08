using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNoCompanyFoundException(long companyId) : AppException(string.Format(GlobalResource.PaymentRequestNoCompanyFound, companyId))
{
    public override string Code => "1001002";
}