using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;

public class TransactionDetailQuery(TransactionDetailRequestViewModel model) : IRequest<Result<TransactionDetailResponseViewModel>>
{
    public TransactionDetailRequestViewModel RequestViewModel { get; set; } = model;
}
