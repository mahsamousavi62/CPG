using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar.WebHook;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetVandarWithdrawalDataCommand(WithdrawalWebhookRequest model) : IRequest<Result<bool>>
{
    public WithdrawalWebhookRequest model { get; set; } = model;
}