namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class SettleTransactionRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string TrackId { get; set; }
    public string ReferenceNumber { get; set; }
    public string ProviderTrackerId { get; set; }
}
