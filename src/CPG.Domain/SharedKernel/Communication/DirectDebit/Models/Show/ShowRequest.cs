namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class ShowRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string MobileNumber { get; set; }
    public string AccessToken { get; set; }
}
