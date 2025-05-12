using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PamentRequestInactiveCompanyException() : AppException(GlobalResource.PaymentRequestCompanyInactive)
{
    public override string Code => "1001001";
}