namespace CPG.Domain.SharedKernel.ApplicationSettings;

public class ApplicationConfigViewModel
{
    public string Payment_Gateway_URL_Prefix { get; set; }
    public string IPG_Callback_URL { get; set; }
    public string CharisPay_BaseUrl { get; set; }
    public string CharisPay_InqueryIbanUrl { get; set; }
    public string CharisPay_Token { get; set; }

}

