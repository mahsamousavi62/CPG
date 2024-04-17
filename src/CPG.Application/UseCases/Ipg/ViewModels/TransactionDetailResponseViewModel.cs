using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class TransactionDetailResponseViewModel
{
    [JsonPropertyName("paymentCode")]
    public string Code { get; set; }

    [JsonPropertyName("trackerId")]
    public string TrackerId { get; set; }

    [JsonPropertyName("destinationDepositIban")]
    public string DestinationDepositIban { get; set; }

    [JsonPropertyName("destinationDepositAccountNumber")]
    public string DestinationDepositAccountNumber { get; set; }
    
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; }

    [JsonPropertyName("paymentMethodType")]
    public short? PaymentMethodType { get; set; }

    [JsonPropertyName("paymentMethodTypeTitle")]
    public string PaymentMethodTypeTitle { get; set; }

    [JsonPropertyName("paymentPattern")]
    public string PaymentPattern { get; set; }

    [JsonPropertyName("status")]
    public short? Status { get; set; }

    [JsonPropertyName("statusTitle")]
    public string StatusTitle { get; set; }

    [JsonPropertyName("predictedExpirationDateTime")]
    public string PredictedExpirationDateTime { get; set; }

    [JsonPropertyName("receiptContent")]
    public string ReceiptContent { get; set; }

    [JsonPropertyName("predictedSettlementDateTime")]
    public string PredictedSettlementDateTime { get; set; }

    [JsonPropertyName("paymentIdentifier")]
    public string PaymentIdentifier { get; set; }
}