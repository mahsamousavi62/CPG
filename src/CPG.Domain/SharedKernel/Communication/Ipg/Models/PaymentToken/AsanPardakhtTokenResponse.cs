using System.Text.Json.Serialization;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class AsanPardakhtTokenResponse : AsanPardakhtResponseBase
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
