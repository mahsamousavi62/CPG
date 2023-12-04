using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Masstransit.Consumer;

internal sealed class FaultConsumer : IConsumer<Fault>
{
    private readonly ILogger<FaultConsumer> _logger;

    public FaultConsumer(ILogger<FaultConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Fault> context)
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