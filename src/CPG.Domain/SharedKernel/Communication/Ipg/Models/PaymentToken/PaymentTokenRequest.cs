namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

public class PaymentTokenRequest
{
    public string ProviderData { get; set; }

    public decimal PaymentRequestAmount { get; set; }
}
