using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class VandarWithdrawalResponse : VandarResponseBase
{
    [JsonPropertyName("result")]
    public WithdrawalResult Result { get; set; }
}

public class WithdrawalResult
{
    [JsonPropertyName("withdrawal")]
    public Withdrawal Withdrawal { get; set; }
}

public class Withdrawal
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("authorization_id")]
    public string AuthorizationId { get; set; }

    [JsonPropertyName("retry_count")]
    public int RetryCount { get; set; }

    [JsonPropertyName("max_retry_count")]
    public int MaxRetryCount { get; set; }

    [JsonPropertyName("gateway_transaction_id")]
    public long GatewayTransactionId { get; set; }

    [JsonPropertyName("refund_id")]
    public object RefundId { get; set; }

    [JsonPropertyName("amount")]
    public string Amount { get; set; }

    [JsonPropertyName("wage_amount")]
    public string WageAmount { get; set; }

    [JsonPropertyName("payment_number")]
    public object PaymentNumber { get; set; }

    [JsonPropertyName("payer_account")]
    public object PayerAccount { get; set; }

    [JsonPropertyName("track_id")]
    public string TrackId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("description")]
    public object Description { get; set; }

    [JsonPropertyName("withdrawal_date")]
    public string WithdrawalDate { get; set; }

    [JsonPropertyName("notify_url")]
    public string NotifyUrl { get; set; }

    [JsonPropertyName("is_instant")]
    public bool IsInstant { get; set; }

    [JsonPropertyName("error_code")]
    public string ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
}