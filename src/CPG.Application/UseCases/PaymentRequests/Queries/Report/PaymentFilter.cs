namespace CPG.Application.UseCases.PaymentRequests.Queries.Report;

public class PaymentFilter
{
    public string PaymentCode { get; set; }
    public long? CompanyId { get; set; }
    public string DestinationDepositIban { get; set; }
    public string NationalCode { get; set; }
    public decimal? Amount { get; set; }
    public string TrackerId { get; set; }
    public Enums.PaymentStatus? Status { get; set; }
    public DateTime? FromUrlExpirationDateTime { get; set; }
    public DateTime? ToUrlExpirationDateTime { get; set; }
    public DateTime? FromCreationDate { get; set; }
    public DateTime? ToCreationDate { get; set; }
    public DateTime? FromModificationDate { get; set; }
    public DateTime? ToModificationDate { get; set; }

}