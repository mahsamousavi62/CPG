using MassTransit;

namespace CPG.Infrastructure.Masstransit.Consumer.UserRegistered;

internal sealed class UserRegisteredConsumerDefinition : ConsumerDefinition<UserRegisteredConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserRegisteredConsumer> consumerConfigurator)
    {
        endpointConfigurator.DiscardFaultedMessages();
        consumerConfigurator.UseMessageRetry(m => m.Immediate(10));
    }
}
