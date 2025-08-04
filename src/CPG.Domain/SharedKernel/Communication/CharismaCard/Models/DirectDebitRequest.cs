using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard.Models;

public class DirectDebitRequest
{
    [JsonPropertyName("amount")]
    [Required]
    public decimal Amount { get; set; }

    [JsonPropertyName("sourceIban")]
    [Required]
    public string SourceIban { get; set; }

    [JsonPropertyName("destinationIban")]
    [Required]
    public string DestinationIban { get; set; }

    [JsonPropertyName("trackerId")]
    [Required]
    public string TrackerId { get; set; }

    [JsonPropertyName("nationalCode")]
    [Required]
    public string NationalCode { get; set; }

    [JsonPropertyName("redirectUrl")]
    [Required]
    public string RedirectUrl { get; set; }
}