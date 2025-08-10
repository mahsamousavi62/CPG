using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard.Models;

public class DirectDebitResponse : CharismaCardBaseResponse<DirectDebitData> { }

public class DirectDebitData
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}



