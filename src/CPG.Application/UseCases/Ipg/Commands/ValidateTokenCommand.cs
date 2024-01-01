using CPG.Application.UseCases.Ipg.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Commands;

public class ValidateTokenCommand(ValidateTokenRequestViewModel model) : IRequest
{
    public ValidateTokenRequestViewModel ValidateToken { get; set; } = model;
}

