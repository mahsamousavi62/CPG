
using System;
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;
public class IPGTransaction : AuditableEntity<long>
{
    public long PaymentRquestId { get; set; }
    public string TrackId { get; set; }
    public short Status { get; set; }
    public bool IsVerified { get; set; }
    public long CompanyIPGId { get; set; }
    public string IPGToken { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ReferenceNumber { get; set; }
    public string EncryptCardNumber { get; set; }
    public int VerificationTimeLimit { get; set; }
    public DateTime PredicateDateTime { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
}

