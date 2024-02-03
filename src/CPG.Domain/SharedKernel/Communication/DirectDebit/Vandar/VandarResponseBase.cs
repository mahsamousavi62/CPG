using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;

public class VandarResponseBase : ResponseBase
{
    [JsonPropertyName("status")]
    public short Status { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}
