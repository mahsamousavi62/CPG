using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Sep;

public class SepResponseBase : ResponseBase
{
    [JsonPropertyName("status")]
    public short Status { get; set; }

    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("errorDesc")]
    public string ErrorDescription { get; set; }
}
