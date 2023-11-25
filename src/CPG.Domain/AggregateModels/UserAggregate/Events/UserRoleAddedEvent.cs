using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events;

public class UserRoleAddedEvent(short roleType) : INotification
{
    public int RoleType { get; set; } = roleType;
}
