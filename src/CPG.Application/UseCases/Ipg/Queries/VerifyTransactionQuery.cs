using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

namespace CPG.Application.UseCases.Ipg.Queries;

public class VerifyTransactionQuery(VerifyTransactionViewModel model) : IRequest<ResultData<VerifyTransactionResponseViewModel>>
{
    public VerifyTransactionViewModel VerifyTransaction { get; set; } = model;
}
