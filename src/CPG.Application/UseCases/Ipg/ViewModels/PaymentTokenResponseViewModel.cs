using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class PaymentTokenResponseViewModel
{
    public string Url { get; set; }

    public dynamic JsonBody { get; set; }

    public Enums.IpgRedirectionMethodType RedirectionMethodType { get; set; }
}
