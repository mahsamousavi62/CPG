namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class PaymentTokenRequest : RequestBase
{
    public string ProviderData { get; set; }
    public decimal PaymentRequestAmount { get; set; }
    public Enums.IpgRedirectionMethodType IpgRedirectionMethodType { get; set; }
    public string SiteAddress { get; set; }
    public string IpgBaseUrl { get; set; }
    public string NationalCode { get; set; }
    public string MobileNumber { get; set; }
    public bool NationalCodeMatchingRequied { get; set; }
    public string ShaparakKey { get; set; }
    public string ShaparakIv { get; set; }
    public int? ThirdPartyCode { get; set; }
    public string PaymentId { get; set; }
}
