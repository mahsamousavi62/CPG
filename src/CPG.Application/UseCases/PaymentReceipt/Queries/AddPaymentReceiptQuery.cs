using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentReceipt.Queries;

public class AddPaymentReceiptQuery(CreatePaymentReceiptModel model) : IRequest<Result<PaymentReceiptResponseViewModel>>
{
    public CreatePaymentReceiptModel viewModel { get; set; } = model;
}