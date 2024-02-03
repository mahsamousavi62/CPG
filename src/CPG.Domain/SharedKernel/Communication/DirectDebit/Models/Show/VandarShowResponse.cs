using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class VandarShowResponse : VandarResponseBase
{
    [JsonPropertyName("data")]
    public List<Datum> Data { get; set; }

    [JsonPropertyName("links")]
    public Link Links { get; set; }

    [JsonPropertyName("meta")]
    public Meta Meta { get; set; }
}

public class Datum
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
    public object PayerAccount { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("revoked_at")]
    public string RevokedAt { get; set; }
}

public class Link
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("first")]
    public string First { get; set; }

    [JsonPropertyName("last")]
    public string Last { get; set; }

    [JsonPropertyName("prev")]
    public object Prev { get; set; }

    [JsonPropertyName("next")]
    public object Next { get; set; }
}

public class Meta
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("from")]
    public int From { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("to")]
    public int To { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}
