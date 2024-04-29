using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyPaymentRequestStatusException() : AppException(string.Format(GlobalResource.VerifyInvalidStatus))
{
    public override string Code => "1015001";
}
