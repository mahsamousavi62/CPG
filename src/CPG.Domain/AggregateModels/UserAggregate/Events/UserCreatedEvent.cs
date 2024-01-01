using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events
{
    public class UserCreatedEvent : INotification
    {
        public User User { get; }

        public UserCreatedEvent(User user) => User = user;
    }
}
