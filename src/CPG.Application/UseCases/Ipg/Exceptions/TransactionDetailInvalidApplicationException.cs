using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class TransactionDetailInvalidApplicationException(long applicationId) : AppException(string.Format(GlobalResource.TransactionDetailInvalidApplication, applicationId))
{
    public override string Code => "1002003";
}
