using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class AyandehTokenResponse : AyandehResponseBase
{
    [JsonPropertyName("traceNumber")]
    public string TraceNumber { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("doTime")]
    public string DoTime { get; set; }
}
