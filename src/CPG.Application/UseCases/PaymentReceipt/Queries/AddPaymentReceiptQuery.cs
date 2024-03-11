using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentReceipt.Queries;

public class AddPaymentReceiptQuery(PaymentReceiptRequestViewModel model) : IRequest<Result<bool>>
{
    public PaymentReceiptRequestViewModel viewModel { get; set; } = model;
}