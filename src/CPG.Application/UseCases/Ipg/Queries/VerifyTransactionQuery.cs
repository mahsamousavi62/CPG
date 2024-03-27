using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;

public class VerifyTransactionQuery(VerifyTransactionViewModel model) : IRequest<Result<VerifyTransactionResponseViewModel>>
{
    public VerifyTransactionViewModel VerifyTransaction { get; set; } = model;
}
