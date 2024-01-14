using CPG.Domain.SharedKernel.Communication.Ipg.Sep;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class SepTokenResponse : SepResponseBase
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
