using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class VerifyTransactionRequest : AsanPardakhtRequestBase
{
    public string ProviderTrackerId { get; set; }

    public string ProviderData { get; set; }
}
