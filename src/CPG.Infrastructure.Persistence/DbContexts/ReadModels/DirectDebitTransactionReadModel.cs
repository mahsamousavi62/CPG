using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class DirectDebitTransactionReadModel
{
    public long Id { get; set; }
    public long DirectDebitGrantId { get; set; }
    public Enums.DirectDebitTransactionStatus Status { get; set; }
    public string TrackId { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ProviderData { get; set; }
    public TransactionReadModel Transaction { get; set; }
    public DirectDebitGrantReadModel DirectDebitGrant { get; set; }
}