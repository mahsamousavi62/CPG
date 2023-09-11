using MediatR;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events
{
    public class DaryaftyarUserReturnedBookEvent : INotification
    {
        public long DaryaftyarUserId { get; }
        public long BookId { get; }

        public DaryaftyarUserReturnedBookEvent(long DaryaftyarUserId, long bookId)
        {
            DaryaftyarUserId = DaryaftyarUserId;
            BookId = bookId;
        }
    }
}