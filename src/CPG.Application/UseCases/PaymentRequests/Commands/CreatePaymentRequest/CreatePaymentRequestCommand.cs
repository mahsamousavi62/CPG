using CPG.Application.UseCases.PaymentRequests.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public record CreatePaymentRequestCommand(CreatePaymentRequestViewModel paymentRequestViewModel) : IRequest<PaymentRequestViewModel>
{
    public CreatePaymentRequestViewModel Model=paymentRequestViewModel;
}
