using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.TransactionAggregate;
public class IPGTransaction : AuditableEntity<long>
{
    public IPGTransaction(string trackId, Enums.IPGTransactionStatus status,
        long companyIPGId, string iPGToken, int verificationTimeLimit)
    {
        TrackId = trackId;
        Status = status;
        CompanyIPGId = companyIPGId;
        IPGToken = iPGToken;
        VerificationTimeLimit = verificationTimeLimit;
        IsActive = true;
    }
    public string TrackId { get; set; }
    public Enums.IPGTransactionStatus Status { get; set; }
    public long CompanyIPGId { get; set; }
    public string IPGToken { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ReferenceNumber { get; set; }
    public string EncryptCardNumber { get; set; }
    public int VerificationTimeLimit { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime? PredicateExpirationDateTime { get; set; }
    public Transaction Transaction { get; set; }

    public static IPGTransaction Create(string trackerId, Enums.IPGTransactionStatus status, long companyIpgId,
        string ipgToken, short verificationTimeLimit)
    {
        var ipgTransaction = new IPGTransaction(trackerId, status, companyIpgId, ipgToken, verificationTimeLimit);
        return ipgTransaction;
    }
}

