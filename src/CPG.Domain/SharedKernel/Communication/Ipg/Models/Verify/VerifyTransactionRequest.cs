namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class VerifyTransactionRequest : RequestBase
{
    public string ProviderTrackerId { get; set; }
    public string ProviderData { get; set; }
    public string Token { get; set; }
}
