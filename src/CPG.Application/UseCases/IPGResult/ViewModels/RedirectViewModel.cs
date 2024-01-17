namespace CPG.Application.UseCases.IPGResult.ViewModels;

public class RedirectViewModel
{
    public string MID { get; set; }
    public long TerminalId { get; set; }
    public long RefNum { get; set; }
    public long ResNum { get; set; }
    public string State { get; set; }
    public string TraceNo { get; set; }
    public long Amount { get; set; }
    public string Wage { get; set; }
    public string Rrn { get; set; }
    public string SecurePan { get; set; }
    public string Token { get; set; }
    public string HashedCardNumber { get; set; }
    public short Status { get; set; }
}
