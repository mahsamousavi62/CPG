using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events;

public class CPGUserBorrowedBookEvent(long cpgUserId, long bookId, DateTimePeriod borrowPeriod) : INotification
{
    public long CPGUserId { get; } = cpgUserId;
    public long BookId { get; } = bookId;
    public DateTimePeriod BorrowPeriod { get; } = borrowPeriod;
}