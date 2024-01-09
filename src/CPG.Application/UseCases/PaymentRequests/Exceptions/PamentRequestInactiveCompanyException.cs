using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PamentRequestInactiveCompanyException(long companyId) : AppException(string.Format(GlobalResource.PaymentRequestCompanyInactive, companyId))
{
    public override string Code => "1001001";
}