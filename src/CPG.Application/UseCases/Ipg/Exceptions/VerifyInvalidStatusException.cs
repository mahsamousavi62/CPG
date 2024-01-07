using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyInvalidStatusException() : ApplicationException(string.Format(GlobalResource.VerifyInvalidStatus))
{
    public override string Code => "1005006";
}
