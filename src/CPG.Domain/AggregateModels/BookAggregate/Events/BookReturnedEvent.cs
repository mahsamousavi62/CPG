using System;
using MediatR;

namespace CPG.Domain.AggregateModels.BookAggregate.Events;

public class BookReturnedEvent(long bookId, DateTime dateOccured) : INotification
{
    public long BookId { get; } = bookId;
    public DateTime DateOccured { get; } = dateOccured;
}
