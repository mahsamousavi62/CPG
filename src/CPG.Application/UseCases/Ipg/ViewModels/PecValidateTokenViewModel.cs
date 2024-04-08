using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class PecValidateTokenViewModel
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    public string Token { get; set; }
    public string OrderId { get; set; }
    public string TerminalNo { get; set; }
    public string RRN { get; set; }
    public string Amount { get; set; }
    public string SwAmount { get; set; }
    public string HashCardNumber { get; set; }
    public string STraceNo { get; set; }
}
