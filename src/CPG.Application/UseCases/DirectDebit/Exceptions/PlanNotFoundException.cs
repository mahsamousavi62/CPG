using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.DirectDebit.Exceptions;

public class PlanNotFoundException(int planId) : AppException(string.Format(GlobalResource.PlanNotFound, planId))
{
    public override string Code => "plan_not_found";
    public int PlanId { get; } = planId;
}