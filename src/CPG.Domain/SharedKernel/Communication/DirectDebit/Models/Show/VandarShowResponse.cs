using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class VandarShowResponse : VandarResponseBase
{
    [JsonPropertyName("result")]
    public Result Result { get; set; }
}

public class Authorizations
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("customer_uuid")]
    public string CustomerUuid { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; }

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("limit")]
    public string Limit { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("national_code")]
    public string NationalCode { get; set; }

    [JsonPropertyName("expiration_date")]
    public string ExpirationDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("payer_account")]
    public PayerAccount PayerAccount { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("revoked_at")]
    public object RevokedAt { get; set; }
}

public class PayerAccount
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; }

    [JsonPropertyName("pan")]
    public string Pan { get; set; }
}

public class Result
{
    [JsonPropertyName("authorizations")]
    public Authorizations Authorizations { get; set; }
}