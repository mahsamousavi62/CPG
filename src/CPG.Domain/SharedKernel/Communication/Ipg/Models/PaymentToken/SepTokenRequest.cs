using CPG.Domain.SharedKernel.Communication.Ipg.Sep;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;

public class SepTokenRequest : SepRequestBase
{
    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("terminalId")]
    public int TerminalId { get; set; }

    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; }

    [JsonPropertyName("resNum")]
    public string ResNum { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("cellNumber")]
    public string CellNumber { get; set; }

    [JsonPropertyName("shaparakKycParams")]
    public ShaparakKycParams ShaparakKycParams { get; set; }
}

public class ShaparakKycParams
{
    [JsonPropertyName("cardHolderNationalId")]
    public string CardHolderNationalId { get; set; }

    [JsonPropertyName("thirdPartyCode")]
    public string ThirdPartyCode { get; set; }
}
