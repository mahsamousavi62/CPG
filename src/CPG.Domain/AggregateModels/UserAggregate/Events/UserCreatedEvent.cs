using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events;

public class UserCreatedEvent(User user) : INotification
{
    public User User { get; } = user;
}
