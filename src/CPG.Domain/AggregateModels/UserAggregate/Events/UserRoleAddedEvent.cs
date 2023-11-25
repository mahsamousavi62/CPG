using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events
{
    public class UserRoleAddedEvent:INotification
    {
        public int RoleType { get; set; }
        public UserRoleAddedEvent(short roleType)
        {
            RoleType = roleType;
        }
    }
}
