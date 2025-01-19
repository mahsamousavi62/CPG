using CPG.Domain.SharedKernel.Communication.Ipg.Sep;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;

public class SepVerifyTransactionResponse : SepResponseBase
{
    [JsonPropertyName("TransactionDetail")]
    public TransactionDetail TransactionDetail { get; set; }

    [JsonPropertyName("PurchaseInfo")]
    public object PurchaseInfo { get; set; }

    [JsonPropertyName("ResultCode")]
    public int ResultCode { get; set; }

    [JsonPropertyName("ResultDescription")]
    public string ResultDescription { get; set; }

    [JsonPropertyName("Success")]
    public bool Success { get; set; }
}

public class TransactionDetail
{
    [JsonPropertyName("RRN")]
    public string RRN { get; set; }

    [JsonPropertyName("RefNum")]
    public string RefNum { get; set; }

    [JsonPropertyName("MaskedPan")]
    public string MaskedPan { get; set; }

    [JsonPropertyName("HashedPan")]
    public string HashedPan { get; set; }

    [JsonPropertyName("TerminalNumber")]
    public int TerminalNumber { get; set; }

    [JsonPropertyName("OrginalAmount")]
    public decimal OrginalAmount { get; set; }

    [JsonPropertyName("AffectiveAmount")]
    public decimal AffectiveAmount { get; set; }

    [JsonPropertyName("StraceDate")]
    public string StraceDate { get; set; }

    [JsonPropertyName("StraceNo")]
    public string StraceNo { get; set; }
}
