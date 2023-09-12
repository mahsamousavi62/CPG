using MediatR;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events
{
    public class DaryaftyarUserReturnedBookEvent : INotification
    {
        public long DaryaftyarUserId { get; }
        public long BookId { get; }

        public DaryaftyarUserReturnedBookEvent(long daryaftyarUserId, long bookId)
        {
            DaryaftyarUserId = daryaftyarUserId;
            BookId = bookId;
        }
    }
}