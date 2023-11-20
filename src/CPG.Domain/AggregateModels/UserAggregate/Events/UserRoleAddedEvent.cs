using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
