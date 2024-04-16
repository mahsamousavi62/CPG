
using CPG.Domain.SharedKernel;
using System;

namespace CPG.Infrastructure.Persistence.GraphQL.Model;

public class CharismaCardTransactionReportViewModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string CompanyPersianName { get; set; }
    public string CompanyEnglishName { get; set; }
    public string CompanyLogo { get; set; }
    public long PaymentRquestId { get; set; }
    public string PaymentCode { get; set; }
    public decimal Amount { get; set; }
    public string ProviderTrackerId { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string TransactionStatusCode { get; set; }
    public Enums.CharismaCardStatus TransactionStatus { get; set; }
    public string TransactionStatusName { get; set; }
    public string TrackerId { get; set; }
}
