using CPG.Domain.SharedKernel.File;
using System;

namespace CPG.Application.UseCases.PaymentReceipt.ViewModels;

public class CreatePaymentReceiptModel(string paymentRequestCode, IFile file, string iban, string receiptIdentifier, long companyDepositId,
    DateTime settlementDateTime, string description)
{
    public string PaymentRequestCode { get; set; } = paymentRequestCode;
    public IFile File { get; set; } = file;
    public string Iban { get; set; } = iban;
    public string ReceiptIdentifier { get; set; } = receiptIdentifier;
    public long CompanyDepositId { get; set; } = companyDepositId;
    public DateTime SettlementDateTime { get; set; } = settlementDateTime;
    public string Description { get; set; } = description;
}
