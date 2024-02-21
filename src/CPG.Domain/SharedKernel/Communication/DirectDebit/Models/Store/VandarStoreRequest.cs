using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class VandarStoreRequest : VandarRequestBase
{
    [JsonProperty("bank_code")]
    public string BankCode { get; set; }

    [JsonProperty("mobile")]
    public string Mobile { get; set; }

    [JsonProperty("callback_url")]
    public string CallbackUrl { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("limit")]
    public decimal Limit { get; set; }

    [JsonProperty("expiration_date")]
    public string ExpirationDate { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("national_code")]
    public string NationalCode { get; set; }
}
