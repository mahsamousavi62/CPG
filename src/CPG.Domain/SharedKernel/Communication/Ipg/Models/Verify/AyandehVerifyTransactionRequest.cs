using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AyandehVerifyTransactionRequest : AyandehRequestBase
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("traceNumber")]
    public string TraceNumber { get; set; }
}
