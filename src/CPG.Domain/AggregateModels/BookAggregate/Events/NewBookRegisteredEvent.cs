using System;
using MediatR;

namespace CPG.Domain.AggregateModels.BookAggregate.Events;

public class NewBookRegisteredEvent(long bookId, DateTime dateOccurred) : INotification
{
    public long BookId { get; } = bookId;
    public DateTime DateOccurred { get; } = dateOccurred;
}
