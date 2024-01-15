using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AsanPardakhtVerifyRequest : AsanPardakhtRequestBase
{
    [JsonPropertyName("merchantConfigurationId")]
    public long MerchantConfigurationId { get; set; }

    [JsonPropertyName("payGateTranId")]
    public string PayGateTranId { get; set; }
}