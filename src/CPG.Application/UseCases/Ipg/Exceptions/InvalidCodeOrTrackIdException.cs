using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.Ipg.Exceptions;

public class InvalidCodeOrTrackIdException(string message) : ApplicationException(string.Format(GlobalResource.RequiredCodeOrTrackIdException, message))
{
    public override string Code => "Invalid_CodeOrTrackId_Exception";
}