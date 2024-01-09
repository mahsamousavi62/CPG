using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyInvalidApplicationException() : AppException(string.Format(GlobalResource.VerifyInvalidApplication))
{
    public override string Code => "1005003";
}
