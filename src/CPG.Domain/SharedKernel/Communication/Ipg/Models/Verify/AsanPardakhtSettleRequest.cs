using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AsanPardakhtSettleRequest : AsanPardakhtRequestBase
{
    [JsonProperty(PropertyName = "merchantConfigurationId")]
    public long MerchantConfigurationId { get; set; }

    [JsonProperty(PropertyName = "payGateTranId")]
    public string PayGateTranId { get; set; }
}
