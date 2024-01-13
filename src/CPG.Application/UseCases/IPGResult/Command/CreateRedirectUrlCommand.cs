using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateRedirectUrlCommand(string Id) : IRequest<Result<string>>
{
    public string MID { get; set; }
    public string TerminalId { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string State { get; set; }
    public string TraceNo { get; set; }
    public decimal Amount { get; set; }
    public string Wage { get; set; }
    public string Rrn { get; set; }
    public string SecurePan { get; set; }
    public string Status { get; set; }
    public string Token { get; set; }
    public string HashedCardNumber { get; set; }
}
