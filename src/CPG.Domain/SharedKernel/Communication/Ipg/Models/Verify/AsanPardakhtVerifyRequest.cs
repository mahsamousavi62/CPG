using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AsanPardakhtVerifyRequest : AsanPardakhtRequestBase
{
    [JsonProperty(PropertyName = "merchantConfigurationId")]
    public long MerchantConfigurationId { get; set; }

    [JsonProperty(PropertyName = "payGateTranId")]
    public string PayGateTranId { get; set; }
}