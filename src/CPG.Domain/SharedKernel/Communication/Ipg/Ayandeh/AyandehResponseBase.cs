using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;

public class AyandehResponseBase : ResponseBase
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
