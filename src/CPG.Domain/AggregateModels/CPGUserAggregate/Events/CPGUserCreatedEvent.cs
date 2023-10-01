using MediatR;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Events
{
    public class CPGUserCreatedEvent : INotification
    {
        public CPGUser User { get; }

        public CPGUserCreatedEvent(CPGUser user) => User = user;
    }
}
