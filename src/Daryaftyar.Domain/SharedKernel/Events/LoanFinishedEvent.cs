using MediatR;

namespace Daryaftyar.Domain.SharedKernel.Events
{
    public class LoanFinishedEvent : INotification
    {
        public long LoanId { get; }

        public LoanFinishedEvent(long loanId)
        {
            LoanId = loanId;
        }
    }
}