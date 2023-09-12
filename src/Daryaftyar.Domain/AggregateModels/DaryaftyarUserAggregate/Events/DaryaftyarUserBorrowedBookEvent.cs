using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events
{
    public class DaryaftyarUserBorrowedBookEvent : INotification
    {
        public long DaryaftyarUserId { get; }
        public long BookId { get; }
        public DateTimePeriod BorrowPeriod { get; }

        public DaryaftyarUserBorrowedBookEvent(long daryaftyarUserId, long bookId, DateTimePeriod borrowPeriod)
        {
            DaryaftyarUserId = daryaftyarUserId;
            BookId = bookId;
            BorrowPeriod = borrowPeriod;
        }
    }
}