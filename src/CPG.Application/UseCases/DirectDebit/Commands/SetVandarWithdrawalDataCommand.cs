using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar.WebHook;
using MediatR;
namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetVandarWithdrawalDataCommand(WithdrawalWebhookRequest model) : IRequest
{
    public WithdrawalWebhookRequest model { get; set; } = model;
}