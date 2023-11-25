using Charisma.MessagingContracts.UsersManagement.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Masstransit.Consumer.UserRegistered
{
    internal sealed class UserRegisteredFaultConsumer : IConsumer<Fault<IUserRegistered>>
    {
        private readonly ILogger<FaultConsumer> _logger;

        public UserRegisteredFaultConsumer(ILogger<FaultConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Fault<IUserRegistered>> context)
        {
            var exceptions = JsonSerializer.Serialize(context.Message.Exceptions, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            _logger.LogError("Message consuming made a fault - {MessageId}, {CorrelationId}, {InitiatorId}, {ExceptionsInfo}",
                context.MessageId, context.CorrelationId, context.InitiatorId, exceptions);

            return Task.CompletedTask;
        }
    }
}
