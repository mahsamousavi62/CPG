using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class PaymentTokenRequest : AsanPardakhtRequestBase
{
    public string ProviderData { get; set; }
    public decimal PaymentRequestAmount { get; set; }
    public Enums.IpgRedirectionMethodType IpgRedirectionMethodType { get; set; }
    public string SiteAddress { get; set; }
    public string IpgBaseUrl { get; set; }
    public string NationalCode { get; set; }
}
