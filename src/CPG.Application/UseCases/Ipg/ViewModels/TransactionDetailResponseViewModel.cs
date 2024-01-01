using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class TransactionDetailResponseViewModel
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("tracker_id")]
    public string TrackerId { get; set; }

    [JsonPropertyName("destination_deposit_iban")]
    public string DestinationDepositIban { get; set; }

    [JsonPropertyName("amount")]
    public string Amount { get; set; }

    [JsonPropertyName("reference_number")]
    public string ReferenceNumber { get; set; }

    [JsonPropertyName("payment_method_type")]
    public string PaymentMethodType { get; set; }

    [JsonPropertyName("payment_method_type_title")]
    public string PaymentMethodTypeTitle { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("status_title")]
    public string StatusTitle { get; set; }

    [JsonPropertyName("predicted_expiration_date_time")]
    public string PredictedExpirationDateTime { get; set; }
}

