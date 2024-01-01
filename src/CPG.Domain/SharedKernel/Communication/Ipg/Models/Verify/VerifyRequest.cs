using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class VerifyRequest : AsanPardakhtRequestBase
{
    [JsonPropertyName("merchantConfigurationId")]
    public long MerchantConfigurationId { get; set; }

    [JsonPropertyName("payGateTranId")]
    public long PayGateTranId { get; set; }
}