using MediatR;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events
{
    public class DaryaftyarUserCreatedEvent : INotification
    {
        public DaryaftyarUser User { get; }

        public DaryaftyarUserCreatedEvent(DaryaftyarUser user) => User = user;
    }
}
