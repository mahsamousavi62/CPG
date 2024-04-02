using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class AyandehVerifyTransactionRequest : AyandehRequestBase
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("traceNumber")]
    public string TraceNumber { get; set; }
}
