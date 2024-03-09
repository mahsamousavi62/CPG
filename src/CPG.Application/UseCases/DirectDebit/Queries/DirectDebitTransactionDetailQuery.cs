using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Queries;

public class DirectDebitTransactionDetailQuery(DirectDebitDetailRequestViewModel model) : IRequest<Result<DirectDebitDetailResponseViewModel>>
{
    public DirectDebitDetailRequestViewModel Model { get; } = model;
}
