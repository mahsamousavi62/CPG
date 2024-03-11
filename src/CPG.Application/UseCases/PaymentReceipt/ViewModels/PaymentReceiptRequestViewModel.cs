using Microsoft.AspNetCore.Http;
using System;

namespace CPG.Application.UseCases.PaymentReceipt.ViewModels;

public class PaymentReceiptRequestViewModel
{
    public string PaymentRequestCode { get; set; }
    public IFormFile File { get; set; }
    public string Iban { get; set; }
    public string ReceiptIdentifier { get; set; }
    public long CompanyDepositId { get; set; }
    public DateTime SettlementDateTime { get; set; }
    public string Description { get; set; }
}
