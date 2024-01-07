using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;

public class ValidateTokenQuery(ValidateTokenRequestViewModel model) : IRequest<Result<ValidateTokenResponseViewModel>>
{
    public ValidateTokenRequestViewModel ValidateToken { get; set; } = model;
}