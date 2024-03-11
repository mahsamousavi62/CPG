using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar.WebHook;

public class WithdrawalWebhookRequest
{
    [JsonPropertyName("withdrawal_id")]
    public string WithdrawalId { get; set; }

    [JsonPropertyName("authorization_id")]
    public string AuthorizationId { get; set; }

    [JsonPropertyName("gateway_transaction_id")]
    public long? GatewayTransactionId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("wage_amount")]
    public string WageAmount { get; set; }

    [JsonPropertyName("payer_account")]
    public PayerAccount? PayerAccount { get; set; }

    [JsonPropertyName("payment_number")]
    public long? PaymentNumber { get; set; }

    [JsonPropertyName("error_code")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
}

public class PayerAccount
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; }

    [JsonPropertyName("pan")]
    public string Pan { get; set; }
}
