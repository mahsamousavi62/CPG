using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.DirectDebit.Exceptions;

public class PlanIsInactiveException() : AppException(GlobalResource.UnexpectedError)
{
    public override string Code => "1009006";
}