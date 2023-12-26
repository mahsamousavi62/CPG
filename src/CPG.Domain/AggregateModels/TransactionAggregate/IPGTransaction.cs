
using System;
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.TransactionAggregate;
public class IPGTransaction : AuditableEntity<long>
{
    public IPGTransaction( string trackId, short status, long companyIPGId, string iPGToken,int verificationTimeLimit)
    {
        TrackId = trackId;
        Status = status;
        CompanyIPGId = companyIPGId;
        IPGToken = iPGToken;
        VerificationTimeLimit = verificationTimeLimit;
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
    public Transaction Transaction { get; set; }

    public static IPGTransaction Create(string trackerId, short status, long companyIpgId, string ipgToken, short verificationTimeLimit)
    {
        var ipgTransaction = new IPGTransaction(trackerId, status, companyIpgId, ipgToken,verificationTimeLimit);
        
        return ipgTransaction;
    }
}

