namespace CPG.Domain.SharedKernel.ApplicationSettings;

public class ApplicationConfigViewModel
{
    public string Payment_Gateway_URL_Prefix { get; set; }
    public string IPG_Callback_URL { get; set; }
    public string CPG_BackEnd { get; set; }
    public string Direct_Debit_Grant_Result_URL { get; set; }
}

