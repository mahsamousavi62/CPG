using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetUserDirectDebitPlanCommand(SetUserDirectDebitPlanViewModel model) : IRequest<Result<bool>>
{
    public SetUserDirectDebitPlanViewModel model { get; set; } = model;
}