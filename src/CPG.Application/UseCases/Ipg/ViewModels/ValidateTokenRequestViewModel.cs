using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;

namespace CPG.Application.UseCases.Ipg.ViewModels;

public class ValidateTokenRequestViewModel
{
    public string Request { get; set; }
}

public class SepValidateTokenViewModel
{
    public string MID { get; set; }
    public string TerminalId { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string State { get; set; }
    public string TraceNo { get; set; }
    public string Amount { get; set; }
    public string Wage { get; set; }
    public string Rrn { get; set; }
    public string SecurePan { get; set; }
    public string Token { get; set; }
    public string HashedCardNumber { get; set; }
    public string Status { get; set; }
}

public class PecValidateTokenViewModel
{
    public short Status { get; set; }
    public long Token { get; set; }
    public long OrderId { get; set; }
    public int TerminalNo { get; set; }
    public long RRN { get; set; }
    public string Amount { get; set; }
    public string SwAmount { get; set; }
    public string HashCardNumber { get; set; }
    public long STraceNo { get; set; }
}
