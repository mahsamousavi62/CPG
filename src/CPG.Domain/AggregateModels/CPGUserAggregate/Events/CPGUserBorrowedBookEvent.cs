using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events
{
    public class CPGUserBorrowedBookEvent : INotification
    {
        public long CPGUserId { get; }
        public long BookId { get; }
        public DateTimePeriod BorrowPeriod { get; }

        public CPGUserBorrowedBookEvent(long cpgUserId, long bookId, DateTimePeriod borrowPeriod)
        {
            CPGUserId = cpgUserId;
            BookId = bookId;
            BorrowPeriod = borrowPeriod;
        }
    }
}