using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard.Models;

public class DirectDebitResponse
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("isFailure")]
    public bool IsFailure { get; set; }

    [JsonPropertyName("data")]
    public DirectDebitData Data { get; set; }

    [JsonPropertyName("error")]
    public CharismaCardError Error { get; set; }
}

public class DirectDebitData
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}