using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class RequiredCodeOrTrackIdException(string message) : AppException(string.Format(GlobalResource.RequiredCodeOrTrackIdException, message))
{
    public override string Code => "Required_CodeOrTrackId_Exception";
}