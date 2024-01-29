namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class PaymentTokenResponse
{
    public string TrackerId { get; set; }

    public string Token { get; set; }

    public string IpgBaseUrl { get; set; }

    public short StatusCode { get; set; }

    public string RefreshToken { get; set; }

    public int ExpiresIn { get; set; }
}