using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class VerifyTransactionResponseViewModel
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
    public decimal Amount { get; set; }

    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; }

    [JsonPropertyName("paymentMethodType")]
    public short PaymentMethodType { get; set; }

    [JsonPropertyName("paymentMethodTypeTitle")]
    public string PaymentMethodTypeTitle { get; set; }

    [JsonPropertyName("status")]
    public short Status { get; set; }

    [JsonPropertyName("statusTitle")]
    public string StatusTitle { get; set; }

    [JsonPropertyName("predictedSettlementDateTime")]
    public string PredictedSettlementDateTime { get; set; }

    [JsonPropertyName("cpgVerificationDateTime")]
    public string CPGVerificationDateTime { get; set; }

    [JsonPropertyName("companyCode")]
    public short? CompanyCode { get; set; }
}
