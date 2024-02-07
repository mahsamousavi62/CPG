using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class VandarShowResponse : VandarResponseBase
{
    [JsonProperty("status")]
    public int GrantStatus { get; set; }

    [JsonProperty("message")]
    public string GrantMessage { get; set; }

    [JsonProperty("result")]
    public Result Result { get; set; }
}

public class Authorizations
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("customer_uuid")]
    public string CustomerUuid { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("bank_code")]
    public string BankCode { get; set; }

    [JsonProperty("callback_url")]
    public string CallbackUrl { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("limit")]
    public string Limit { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("national_code")]
    public string NationalCode { get; set; }

    [JsonProperty("expiration_date")]
    public string ExpirationDate { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("payer_account")]
    public PayerAccount PayerAccount { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("revoked_at")]
    public object RevokedAt { get; set; }
}

public class PayerAccount
{
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }

    [JsonProperty("pan")]
    public string Pan { get; set; }
}

public class Result
{
    [JsonProperty("authorizations")]
    public Authorizations Authorizations { get; set; }
}