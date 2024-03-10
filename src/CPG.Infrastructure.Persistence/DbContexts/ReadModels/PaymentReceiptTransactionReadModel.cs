using CPG.Domain.AggregateModels.TransactionAggregate;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class PaymentReceiptTransactionReadModel
{
    public long Id { get; set; }
    public string SourceIban { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime ReceiptDateTime { get; set; }
    public string Description { get; set; }
    public string ReceiptImage { get; set; }
    public PaymentReceiptStatus Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public TransactionReadModel Transaction { get; set; }
}
