using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;

public class VandarResponseBase : ResponseBase
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("status")]
    public short Status { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; }
}
