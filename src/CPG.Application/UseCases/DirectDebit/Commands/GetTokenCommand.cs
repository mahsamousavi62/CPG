using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetTokenCommand(GetTokenViewModel model) : IRequest<Result<bool>>
{
    public GetTokenViewModel model { get; set; } = model;
}
