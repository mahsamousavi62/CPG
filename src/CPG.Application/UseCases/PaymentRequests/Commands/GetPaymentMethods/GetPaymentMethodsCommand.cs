using CPG.Application.UseCases.PaymentRequests.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommand(GetPaymentMethodsViewModel viewModel) : IRequest<PaymentMethodsViewModel>
{
    public GetPaymentMethodsViewModel ViewModel = viewModel;
}
