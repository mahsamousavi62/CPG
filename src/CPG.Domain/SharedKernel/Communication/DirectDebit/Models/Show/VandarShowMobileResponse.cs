using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class VandarShowMobileResponse : VandarResponseBase
{
    [JsonProperty("message")]
    public string GrantMessage { get; set; }

    [JsonProperty("data")]
    public List<Datum> Data { get; set; }

    [JsonProperty("links")]
    public Link Links { get; set; }

    [JsonProperty("meta")]
    public Meta Meta { get; set; }
}

public class Datum
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
    public decimal Limit { get; set; }

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
    public PayerAccountData PayerAccount { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    [JsonProperty("revoked_at")]
    public string RevokedAt { get; set; }
}

public class Link
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("first")]
    public string First { get; set; }

    [JsonProperty("last")]
    public string Last { get; set; }

    [JsonProperty("prev")]
    public object Prev { get; set; }

    [JsonProperty("next")]
    public object Next { get; set; }
}

public class Meta
{
    [JsonProperty("current_page")]
    public int CurrentPage { get; set; }

    [JsonProperty("from")]
    public int From { get; set; }

    [JsonProperty("last_page")]
    public int LastPage { get; set; }

    [JsonProperty("links")]
    public List<Link> Links { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }

    [JsonProperty("to")]
    public int To { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }
}

public class PayerAccountData
{
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }

    [JsonProperty("pan")]
    public string Pan { get; set; }
}
