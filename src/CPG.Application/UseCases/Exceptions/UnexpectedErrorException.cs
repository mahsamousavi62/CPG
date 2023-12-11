using CPG.Application.Shared.Resource;

namespace CPG.Application.UseCases.Exceptions;

internal class UnexpectedErrorException() : ApplicationException(GlobalResource.UnexpectedError)
{
    public override string Code => "1006000";
}