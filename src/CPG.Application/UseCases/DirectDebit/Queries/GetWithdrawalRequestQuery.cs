using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Queries;

public class GetWithdrawalRequestQuery(WithdrawalRequestViewModel model) : IRequest<Result<bool>>
{
    public WithdrawalRequestViewModel PaymentToken { get; set; } = model;
}