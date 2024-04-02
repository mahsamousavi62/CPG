using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AyandehVerifyTransactionResponse : AyandehResponseBase
{
    [JsonProperty("serviceAmountList")]
    public List<ServiceAmount> ServiceAmountList { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("rrn")]
    public string Rrn { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}

public class ServiceAmount
{
    [JsonProperty("serviceId")]
    public int ServiceId { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}
