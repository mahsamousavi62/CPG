using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class TransactionDetailRequestViewModel
{
    [JsonPropertyName("paymentCode")]
    public string Code { get; set; }

    [JsonPropertyName("trackerId")]
    public string TrackerId { get; set; }
}
