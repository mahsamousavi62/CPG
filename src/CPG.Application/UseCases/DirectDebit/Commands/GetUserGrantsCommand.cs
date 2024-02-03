using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetUserGrantsCommand(GetUserGrantsViewModel model) : IRequest<Result<bool>>
{
    public GetUserGrantsViewModel model { get; set; } = model;
}
