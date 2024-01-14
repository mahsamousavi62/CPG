using CPG.Domain.SharedKernel.Communication.Ipg.Sep;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class SepVerifyTransactionRequest : SepRequestBase
{
    [JsonPropertyName("terminalNumber")]
    public long TerminalNumber { get; set; }

    [JsonPropertyName("refNum")]
    public string RefNum { get; set; }
}
