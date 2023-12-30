using CPG.Application.UseCases.Ipg.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Commands;

public class ValidateTokenCommand(ValidateTokenViewModel model) : IRequest
{
    public ValidateTokenViewModel ValidateToken { get; set; } = model;
}

