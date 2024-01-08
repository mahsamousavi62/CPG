using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.Ipg.Exceptions;

public class InvalidCodeOrTrackIdException(string message) : AppException(string.Format(GlobalResource.InvalidCodeOrTrackIdException, message))
{
    public override string Code => "Invalid_CodeOrTrackId_Exception";
}