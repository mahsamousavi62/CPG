using CPG.Application.UseCases.Exceptions;
using CPG.Application.Shared.Resource;

namespace CPG.Application.UseCases.Ipg.Exceptions;

public class TrackIdInvalidStatusException() : ApplicationException(GlobalResource.TrackIdInvalidStatusException)
{
    public override string Code => "1008002";
}