using System;
using MediatR;

namespace CPG.Domain.AggregateModels.BookAggregate.Events
{
    public class NewBookRegisteredEvent : INotification
    {
        public long BookId { get; }
        public DateTime DateOccurred { get; }

        public NewBookRegisteredEvent(long bookId, DateTime dateOccurred)
        {
            BookId = bookId;
            DateOccurred = dateOccurred;
        }
    }
}
