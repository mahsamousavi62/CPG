using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetDirectDebitPlansCommand(GetDirectDebitPlansViewModel model) : IRequest<Result<PlanViewModel>>
{
    public GetDirectDebitPlansViewModel model { get; set; } = model;
}
