using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommand(GetPaymentMethodsViewModel viewModel) : IRequest<Result<PaymentMethodsViewModel>>
{
    public GetPaymentMethodsViewModel ViewModel = viewModel;
}
