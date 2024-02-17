using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class ConfirmGrantCommand(ConfirmGrantViewModel model) : IRequest<Result<ConfirmGrantResponseViewModel>>
{
    public ConfirmGrantViewModel model { get; set; } = model;
}