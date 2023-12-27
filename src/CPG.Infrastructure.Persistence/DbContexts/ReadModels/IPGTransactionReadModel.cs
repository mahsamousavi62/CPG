
using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.TransactionAggregate;
public class IPGTransactionReadModel
{
    public long Id { get; set; }
    public string TrackId { get; set; }
    public Enums.IPGTransactionStatus Status { get; set; }
    public long CompanyIPGId { get; set; }
    public string IPGToken { get; set; }
    public string ProviderTrackerId { get; set; }
    public string ReferenceNumber { get; set; }
    public string EncryptCardNumber { get; set; }
    public int VerificationTimeLimit { get; set; }
    public DateTime PredicateDateTime { get; set; }
    public TransactionReadModel Transaction { get; set; }
}

