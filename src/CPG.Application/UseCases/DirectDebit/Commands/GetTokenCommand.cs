using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetTokenCommand(PaymentTokenViewModel model) : IRequest<Result<PaymentTokenResponseViewModel>>
{
    public PaymentTokenViewModel PaymentToken { get; set; } = model;
}
