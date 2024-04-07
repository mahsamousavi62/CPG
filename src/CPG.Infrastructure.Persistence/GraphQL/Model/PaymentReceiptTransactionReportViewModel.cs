

using System;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.GraphQL.Model;

public class PaymentReceiptTransactionReportViewModel
{
    public int TotalCount { get; set; }
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string CompanyPersianName { get; set; }
    public string CompanyEnglishName { get; set; }
    public string CompanyLogo { get; set; }
    public long PaymentRquestId { get; set; }
    public string PaymentCode { get; set; }
    public long PaymentReceiptTransactionId { get; set; }
    public decimal Amount { get; set; }
    public long BankId { get; internal set; }
    public string BankName { get; set; }
    public string BankLogo { get; set; }
    public string SourceIban { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime ReceiptDateTime { get; set; }
    public string Description { get; set; }
    public string ReceiptImage { get; set; }
    public PaymentReceiptStatus TransactionStatus { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public string TransactionStatusCode { get; set; }
    public string TransactionStatusName { get;  set; }
}
