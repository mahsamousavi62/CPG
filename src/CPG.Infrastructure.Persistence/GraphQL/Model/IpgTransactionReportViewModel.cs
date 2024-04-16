

using CPG.Domain.SharedKernel;
using System;

namespace CPG.Infrastructure.Persistence.GraphQL.Model;

public class IpgTransactionReportViewModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string CompanyPersianName { get; set; }
    public string CompanyEnglishName { get; set; }
    public string CompanyLogo { get; set; }
    public long PaymentRquestId { get; set; }
    public string PaymentCode { get; set; }
    public long IPGTransactionId { get; set; }
    public decimal Amount { get; set; }
    public long? IPGTypeId { get; internal set; }
    public string IpgTypePersianName { get; set; }
    public string IpgTypeLogo { get; set; }
    public string IPGToken { get; set; }
    public string ProviderTrackerId { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime? PredicateExpirationDateTime { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime? CreationDate { get; set; }
    public string TransactionStatusCode { get;  set; }
    public Enums.IPGTransactionStatus TransactionStatus { get;  set; }
    public string TransactionStatusName { get;  set; }
    public string TrackerId { get;  set; }
    public DateTime? PredictedSettlementDateTime { get; set; }
}
