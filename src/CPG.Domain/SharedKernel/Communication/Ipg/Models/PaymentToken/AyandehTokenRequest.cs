using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class AyandehTokenRequest : AyandehRequestBase
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("additionalData")]
    public string AdditionalData { get; set; }

    [JsonPropertyName("callBackUrl")]
    public string CallBackUrl { get; set; }

    [JsonPropertyName("amount")]
    public string Amount { get; set; }

    [JsonPropertyName("serviceId")]
    public string ServiceId { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("settleDate")]
    public string SettleDate { get; set; }
}
