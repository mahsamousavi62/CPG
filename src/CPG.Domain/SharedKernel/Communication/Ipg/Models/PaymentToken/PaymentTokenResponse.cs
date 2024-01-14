namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class PaymentTokenResponse
{
    public short Status { get; set; }

    public string TrackerId { get; set; }

    public string Token { get; set; }

    public string IpgBaseUrl { get; set; }
}