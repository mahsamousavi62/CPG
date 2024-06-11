using System;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.GraphQL.Model;

public class PaymentRequestReportViewModel
{
    public long Id { get; set; }
    public string PaymentCode { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; }
    public long ApplicationId { get; set; }
    public string DestinationDepositIban { get; set; }
    public string NationalCode { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string TrackerId { get; set; }
    public string PaymentIdentifier { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public string StatusName { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime UrlExpirationDateTime { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public string StatusCode { get; internal set; }
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
}
