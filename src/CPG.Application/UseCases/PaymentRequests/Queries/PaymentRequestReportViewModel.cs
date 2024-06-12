namespace CPG.Application.UseCases.PaymentRequests.Queries;

public class PaymentRequestReportViewModel
{
    public string PaymentCode { get; set; }
    public long CompanyId { get; set; }
    public string CompanyTitle { get; set; }
    public string DestinationDepositIban { get; set; }
    public string AccountNumber { get; set; }
    public string NationalCode { get; set; }
    public string UserFullName { get; set; }
    public decimal Amount { get; set; }
    public string TrackerId { get; set; }
    public string PaymentIdentifier { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public string StatusPersianName { get; set; }
    public string StatusEnglishName { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime UrlExpirationDateTime { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }

}
