using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class NotFoundTrackIdException() : AppException(GlobalResource.NotFoundTrackIdException)
{
    public override string Code => "1008001";
}