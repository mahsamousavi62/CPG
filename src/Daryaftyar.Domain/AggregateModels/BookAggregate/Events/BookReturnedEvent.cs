using System;
using MediatR;

namespace Daryaftyar.Domain.AggregateModels.BookAggregate.Events
{
    public class BookReturnedEvent : INotification
    {
        public long BookId { get; }
        public DateTime DateOccured { get; }

        public BookReturnedEvent(long bookId, DateTime dateOccured)
        {
            BookId = bookId;
            DateOccured = dateOccured;
        }
    }
}
