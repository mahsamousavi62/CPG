using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class VandarWithdrawalRequest : VandarRequestBase
{
    [JsonPropertyName("authorization_id")]
    public string AuthorizationId { get; set; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("withdrawal_date")]
    public string WithdrawalDate { get; set; }
    [JsonPropertyName("is_instant")]
    public bool IsInstant { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("max_retry_count")]
    public short MaxRetryCount { get; set; }
    [JsonPropertyName("track_id")]
    public string TrackId { get; set; }
    [JsonPropertyName("notify_url")]
    public string NotifyUrl { get; set; }
}
