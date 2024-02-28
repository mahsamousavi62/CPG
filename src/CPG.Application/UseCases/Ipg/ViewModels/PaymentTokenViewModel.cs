namespace CPG.Application.UseCases.Ipg.ViewModels;

public class PaymentTokenViewModel
{
    public long? CompanyIPGId { get; set; }
    public string PaymentRequestCode { get; set; }
    public short? CompanyPaymentMethodType { get; set; }
    public long? GrantId { get; set; }
}
