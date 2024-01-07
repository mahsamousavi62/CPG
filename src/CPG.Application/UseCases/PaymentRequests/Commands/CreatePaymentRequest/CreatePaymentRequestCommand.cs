using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public record CreatePaymentRequestCommand(CreatePaymentRequestViewModel paymentRequestViewModel) : IRequest<Result<PaymentRequestResponseViewModel>>
{
    public CreatePaymentRequestViewModel Model=paymentRequestViewModel;
}
