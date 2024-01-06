using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyInvalidCodeOrTrackIdException() : ApplicationException(string.Format(GlobalResource.InvalidCodeOrTrackIdException))
{
    public override string Code => "1005002";
}