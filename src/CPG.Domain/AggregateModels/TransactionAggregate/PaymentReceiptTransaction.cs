using System;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class PaymentReceiptTransaction : AuditableEntity<long>
{
    public PaymentReceiptTransaction()
    {
        
    }
    public PaymentReceiptTransaction(Iban sourceIban, string referenceNumber, DateTime receiptDateTime, string description, Logo receiptImage,
        PaymentReceiptStatus status)
    {
        SourceIban = sourceIban.Value;
        ReferenceNumber = referenceNumber;
        ReceiptDateTime = receiptDateTime;
        Description = description;
        ReceiptImage = receiptImage.Value;
        Status = status;
    }

    public string SourceIban { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime ReceiptDateTime { get; set; }
    public string Description { get; set; }
    public string ReceiptImage { get; set; }
    public PaymentReceiptStatus Status { get; set; }
    public Transaction Transaction { get; set; }

    public static PaymentReceiptTransaction Create(Iban sourceIban, string referenceNumber, DateTime receiptDateTime, string description,
        Logo receiptImage, PaymentReceiptStatus status)
    {
        var paymentReceipt = new PaymentReceiptTransaction(sourceIban, referenceNumber, receiptDateTime, description, receiptImage, status);
        return paymentReceipt;
    }
}
