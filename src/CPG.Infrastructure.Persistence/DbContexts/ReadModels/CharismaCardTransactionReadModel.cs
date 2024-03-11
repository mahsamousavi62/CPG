

using CPG.Domain.AggregateModels.TransactionAggregate;
using System;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CharismaCardTransactionReadModel
{
    public long Id { get; set; }
    public string TrackId { get; set; }
    public string ProviderTrackId { get; set; }
    public string ReferenceNumber { get; set; }
    public Enums.CharismaCardStatus Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public TransactionReadModel Transaction { get; set; }
}
