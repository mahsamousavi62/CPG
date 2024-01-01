
using Charisma.MessagingContracts.UsersManagement.User;
using MassTransit;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Masstransit.Consumer.UserRegistered;

internal sealed class UserRegisteredConsumer : IConsumer<IUserRegistered>
{
    public UserRegisteredConsumer()
    {

    }

    Task IConsumer<IUserRegistered>.Consume(ConsumeContext<IUserRegistered> context)
    {
        var phoneNumber = context.Message.PhoneNumber;

        return Task.CompletedTask;
    }
}
