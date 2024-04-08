using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class AyandehValidateTokenViewModel
{
    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("desc")]
    public string Desc { get; set; }

    [JsonPropertyName("traceNumber")]
    public string TraceNumber { get; set; }

    [JsonPropertyName("channelRefNumber")]
    public string ChannelRefNumber { get; set; }

    [JsonPropertyName("additionalData")]
    public string AdditionalData { get; set; }
}
