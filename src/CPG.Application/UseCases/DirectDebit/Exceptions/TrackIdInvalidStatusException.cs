using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.DirectDebit.Exceptions;

public class TrackIdInvalidStatusException() : AppException(GlobalResource.TrackIdInvalidStatusException)
{
    public override string Code => "1010002";
}