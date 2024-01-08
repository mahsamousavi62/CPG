using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class VerifyRequiredCodeOrTrackIdException() : AppException(string.Format(GlobalResource.RequiredCodeOrTrackIdException))
{
    public override string Code => "1005001";
}