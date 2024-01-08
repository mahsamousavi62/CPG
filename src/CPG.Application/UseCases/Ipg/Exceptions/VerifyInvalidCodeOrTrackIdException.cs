using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyInvalidCodeOrTrackIdException() : AppException(string.Format(GlobalResource.InvalidCodeOrTrackIdException))
{
    public override string Code => "1005002";
}