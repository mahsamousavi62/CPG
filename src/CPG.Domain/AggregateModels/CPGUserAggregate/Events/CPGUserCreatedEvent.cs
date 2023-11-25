using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events;

public class CPGUserCreatedEvent(CPGUser user) : INotification
{
    public CPGUser User { get; } = user;
}
