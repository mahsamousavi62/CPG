using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class DirectDebitTransaction : AuditableEntity<long>
{
    public DirectDebitTransaction(long directDebitGrantId, Enums.DirectDebitTransactionStatus status, string trackId,
         string providerTrackerId, string providerData)
    {
        DirectDebitGrantId = directDebitGrantId;
        Status = status;
        TrackId = trackId;
        ProviderTrackerId = providerTrackerId;
        ProviderData = providerData;
        IsActive = true;
    }
    public long DirectDebitGrantId { get; set; }
    public Enums.DirectDebitTransactionStatus Status { get; set; }
    public string TrackId { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ProviderData { get; set; }    
    public Transaction Transaction { get; set; }
    public DirectDebitGrant DirectDebitGrant { get; set; }

    public static DirectDebitTransaction Create(long directDebitGrantId, Enums.DirectDebitTransactionStatus status, string trackerId,
        string providerTrackerId, string providerData)
    {
        var directDebitTransaction = new DirectDebitTransaction(directDebitGrantId, status, trackerId, providerTrackerId, providerData);
        return directDebitTransaction;
    }
}