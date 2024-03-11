using CPG.Domain.SharedKernel.File;
using System;

namespace CPG.Application.UseCases.PaymentReceipt.ViewModels;

public class PaymentReceiptRequestViewModel
{
    public string PaymentRequestCode { get; set; }
    public IFile File { get; set; }
    public string Iban { get; set; }
    public string ReceiptIdentifier { get; set; }
    public long CompanyDepositId { get; set; }
    public DateTime SettlementDateTime { get; set; }
}
