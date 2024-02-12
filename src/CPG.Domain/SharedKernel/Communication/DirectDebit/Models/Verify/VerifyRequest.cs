namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;

public class VerifyRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string AccessToken { get; set; }
    public string AuthorizationId { get; set; }
}
