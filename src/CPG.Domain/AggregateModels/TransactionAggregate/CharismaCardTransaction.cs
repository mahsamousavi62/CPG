using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class CharismaCardTransaction : AuditableEntity<long>    
{
    public CharismaCardTransaction(string trackId, string providerTrackId, string referenceNumber, CharismaCardStatus status)
    {
        TrackId = trackId;
        ProviderTrackId = providerTrackId;
        ReferenceNumber = referenceNumber;
        Status = status;
    }
    public CharismaCardTransaction()
    {
            
    }
    public string TrackId { get; set; }
    public string ProviderTrackId { get; set; }
    public string ReferenceNumber { get; set; }
    public Enums.CharismaCardStatus Status { get; set; }
    public Transaction Transaction { get; set; }

    public static CharismaCardTransaction Create(string trackId, string providerTrackId, string referenceNumber, CharismaCardStatus status)
    {
        var charismaCard = new CharismaCardTransaction(trackId,providerTrackId, referenceNumber, status);
        return charismaCard;
    }

}
