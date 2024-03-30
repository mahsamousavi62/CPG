using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class AyandehTokenRequest : AyandehRequestBase
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("additionalData")]
    public string AdditionalData { get; set; }

    [JsonProperty("callBackUrl")]
    public string CallBackUrl { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("serviceId")]
    public string ServiceId { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }

    [JsonProperty("settleDate")]
    public string SettleDate { get; set; }
}
