using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events
{
    public class CPGUserReturnedBookEvent : INotification
    {
        public long CPGUserId { get; }
        public long BookId { get; }

        public CPGUserReturnedBookEvent(long cpgUserId, long bookId)
        {
            CPGUserId = cpgUserId;
            BookId = bookId;
        }
    }
}