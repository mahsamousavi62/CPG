using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel;
using MediatR;
using CPG.Application.UseCases.Ipg.ViewModels;

namespace CPG.Application.UseCases.Ipg.Commands;

public class GetPaymentTokenCommand(PaymentTokenViewModel model) : IRequest<Result<PaymentTokenResponse>>
{
    public PaymentTokenViewModel PaymentToken { get; set; } = model;
}
