using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events;

public class CPGUserReturnedBookEvent(long cpgUserId, long bookId) : INotification
{
    public long CPGUserId { get; } = cpgUserId;
    public long BookId { get; } = bookId;
}