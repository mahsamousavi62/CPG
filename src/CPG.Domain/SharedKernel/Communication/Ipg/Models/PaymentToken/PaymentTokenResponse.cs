namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class Params
{
    public string Token { get; set; }
}

public class PaymentTokenResponse
{
    public string Url { get; set; }
    public string Method => "Post";
    public Params Params { get; set; }
    public string TrackerId { get; set; }
}
