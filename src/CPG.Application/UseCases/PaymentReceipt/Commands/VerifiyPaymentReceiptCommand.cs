using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.PaymentReceipt.Commands;

public class VerifiyPaymentReceiptCommand(VerifyPaymentReceiptTransactionViewModel model) : IRequest<Result<string>>
{
    public VerifyPaymentReceiptTransactionViewModel VerifyTransaction { get; set; } = model;
}
