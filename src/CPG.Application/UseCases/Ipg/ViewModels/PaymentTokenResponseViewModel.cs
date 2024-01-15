using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class PaymentTokenResponseViewModel
{
    public string Url { get; set; }

    public dynamic JsonBody { get; set; }

    public string Method => "Post";

    public Enums.IpgRedirectionMethodType RedirectionMethodType { get; set; }
}
