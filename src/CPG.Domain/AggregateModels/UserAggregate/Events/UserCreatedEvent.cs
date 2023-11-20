using CPG.Domain.AggregateModels.UserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.UserAggregate.Events
{
    public class UserCreatedEvent : INotification
    {
        public User User { get; }

        public UserCreatedEvent(User user) => User = user;
    }
}
