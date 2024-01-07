using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class TransactionDetailInvalidApplicationException() : ApplicationException(string.Format(GlobalResource.TransactionDetailInvalidApplication))
{
    public override string Code => "1002003";
}
