using System;
using CPG.Domain.SeedWork;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class PaymentReceiptTransaction : AuditableEntity<long>
{
    public PaymentReceiptTransaction(string sourceIban, string referenceNumber, DateTime receiptDateTime, string description, string receiptImage,
        PaymentReceiptStatus status)
    {
        SourceIban = sourceIban;
        ReferenceNumber = referenceNumber;
        ReceiptDateTime = receiptDateTime;
        Description = description;
        ReceiptImage = receiptImage;
        Status = status;
    }

    public string SourceIban { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime ReceiptDateTime { get; set; }
    public string Description { get; set; }
    public string ReceiptImage { get; set; }
    public PaymentReceiptStatus Status { get; set; }
    public Transaction Transaction { get; set; }

    public static PaymentReceiptTransaction Create(string sourceIban, string referenceNumber, DateTime receiptDateTime, string description,
        string receiptImage, PaymentReceiptStatus status)
    {
        var paymentReceipt = new PaymentReceiptTransaction(sourceIban, referenceNumber, receiptDateTime, description, receiptImage, status);
        return paymentReceipt;
    }
}
