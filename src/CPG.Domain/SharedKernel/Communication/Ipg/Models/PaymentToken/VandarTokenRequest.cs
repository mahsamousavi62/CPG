using CPG.Domain.SharedKernel.Communication.Ipg.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class VandarTokenRequest : VandarRequestBase
{
    [JsonPropertyName("refreshtoken")]
    public string RefreshToken { get; set; }
}
