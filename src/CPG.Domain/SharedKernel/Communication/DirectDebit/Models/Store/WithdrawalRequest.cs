namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class WithdrawalRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string AccessToken { get; set; }
}
