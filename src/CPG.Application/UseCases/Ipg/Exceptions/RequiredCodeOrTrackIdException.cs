using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class RequiredCodeOrTrackIdException() : AppException(GlobalResource.RequiredCodeOrTrackIdException)
{
    public override string Code => "Required_CodeOrTrackId_Exception";
}