using MediatR;

namespace CPG.Domain.SharedKernel.Events;

public class LoanFinishedEvent(long loanId) : INotification
{
    public long LoanId { get; } = loanId;
}