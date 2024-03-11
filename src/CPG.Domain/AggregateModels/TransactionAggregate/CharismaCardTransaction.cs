using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class CharismaCardTransaction : AuditableEntity<long>    
{
    public string TrackId { get; set; }
    public string ProviderTrackId { get; set; }
    public string ReferenceNumber { get; set; }
    public Enums.CharismaCardStatus Status { get; set; }

}
