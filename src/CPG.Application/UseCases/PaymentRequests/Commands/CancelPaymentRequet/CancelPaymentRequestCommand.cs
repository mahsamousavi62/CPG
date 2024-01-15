using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CancelPaymentRequet
{
    public class CancelPaymentRequestCommand(CancelPaymentRequestViewModel viewModel) :IRequest<Result<CancelPaymentRequestResponseViewModel>>
    {
        public CancelPaymentRequestViewModel ViewModel = viewModel;
    }
}
