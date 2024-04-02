using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AyandehVerifyTransactionResponse : AyandehResponseBase
{
    [JsonPropertyName("serviceAmountList")]
    public List<ServiceAmount> ServiceAmountList { get; set; }

    [JsonPropertyName("amount")]
    public string Amount { get; set; }

    [JsonPropertyName("rrn")]
    public string Rrn { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class ServiceAmount
{
    [JsonPropertyName("serviceId")]
    public int ServiceId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
