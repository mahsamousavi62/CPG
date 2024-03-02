
using System;
using System.Buffers;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.PaymentReceiptAggregate;

public class PaymentReceiptTransaction : AuditableEntity<long>, IAggregateRoot
{
    public string SourceIban { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime ReceiptDateTime { get; set; }
    public string Description { get; set; }
    public string ReceiptImage { get; set; }
    public Enums.PaymentReceiptStatus Status { get; set; }
}
