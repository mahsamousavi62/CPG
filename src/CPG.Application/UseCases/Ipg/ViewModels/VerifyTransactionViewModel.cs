using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class VerifyTransactionViewModel
{
    [JsonPropertyName("paymentCode")]
    public string Code { get; set; }

    [JsonPropertyName("trackerId")]
    public string TrackerId { get; set; }
}
