using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Domain.AggregateModels.BookAggregate.Events;

public class BookBorrowedEvent(long bookId, long userId, DateTimePeriod dateTimePeriod) : INotification
{
    public long BookId { get; } = bookId;
    public long UserId { get; } = userId;
    public DateTimePeriod DateTimePeriod { get; } = dateTimePeriod;
}
