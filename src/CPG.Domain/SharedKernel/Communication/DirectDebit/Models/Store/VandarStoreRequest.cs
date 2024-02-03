using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class VandarStoreRequest : VandarRequestBase
{
    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("limit")]
    public decimal Limit { get; set; }

    [JsonPropertyName("expiration_date")]
    public string ExpirationDate { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("national_code")]
    public string NationalCode { get; set; }
}
