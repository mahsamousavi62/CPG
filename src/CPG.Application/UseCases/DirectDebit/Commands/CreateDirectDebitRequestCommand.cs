using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class CreateDirectDebitRequestCommand(CreateDirectDebitRequestViewModel model) : IRequest<Result<bool>>
{
    public CreateDirectDebitRequestViewModel model { get; set; } = model;
}
