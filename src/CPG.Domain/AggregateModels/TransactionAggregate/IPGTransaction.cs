
using System;
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.TransactionAggregate;
public class IPGTransaction : AuditableEntity<long>
{
    public IPGTransaction( string trackId, short status, long companyIPGId, string iPGToken, string providerTrackerId, string referenceNumber, string encryptCardNumber, int verificationTimeLimit, DateTime predicateDateTime)
    {
        TrackId = trackId;
        Status = status;
        CompanyIPGId = companyIPGId;
        IPGToken = iPGToken;
        ProviderTrackerId = providerTrackerId;
        ReferenceNumber = referenceNumber;
        EncryptCardNumber = encryptCardNumber;
        VerificationTimeLimit = verificationTimeLimit;
        PredicateDateTime = predicateDateTime;
    }
    public string TrackId { get; set; }
    public short Status { get; set; }
    public long CompanyIPGId { get; set; }
    public string IPGToken { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ReferenceNumber { get; set; }
    public string EncryptCardNumber { get; set; }
    public int VerificationTimeLimit { get; set; }
    public DateTime PredicateDateTime { get; set; }
    //public Transaction Transaction { get; set; }
}

