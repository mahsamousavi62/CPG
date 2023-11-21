using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users.EventHandlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly IAggregateRepository<User> repository;

        public UserCreatedEventHandler(IAggregateRepository<User> repository)
        {
            this.repository = repository;
        }
        public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {

        }
    }
}
