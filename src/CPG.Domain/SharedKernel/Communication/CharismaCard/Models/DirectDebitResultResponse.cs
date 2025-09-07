using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard.Models;

public class DirectDebitResultResponse : CharismaCardBaseResponse<DirectDebitResultData> { }

public class DirectDebitResultData
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("sourceIban")]
    public string SourceIban { get; set; }

    [JsonPropertyName("destinationIban")]
    public string DestinationIban { get; set; }

    [JsonPropertyName("trackerId")]
    public string TrackerId { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }
}