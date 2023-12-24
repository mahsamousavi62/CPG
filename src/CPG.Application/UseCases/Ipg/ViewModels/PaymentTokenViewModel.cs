namespace CPG.Application.UseCases.Ipg.ViewModels;

public class PaymentTokenViewModel
{
    public long CompanyIPGId { get; set; }
    public long PaymentRequestId { get; set; }
    public short CompanyPaymentMethodType { get; set; }
}
