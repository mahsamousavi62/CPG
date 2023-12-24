using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

public class AsanPardakhtTokenResponse : AsanPardakhtResponseBase
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
