using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;

public class ValidateTokenQuery(ValidateTokenViewModel model) : IRequest<ResultData<string>>
{
    public ValidateTokenViewModel ValidateToken { get; set; } = model;
}