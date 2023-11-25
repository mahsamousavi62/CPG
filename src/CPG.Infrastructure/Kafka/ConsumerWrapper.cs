using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace CPG.Infrastructure.Kafka;

public class ConsumerWrapper
{
    private readonly ILogger<ConsumerWrapper> _logger;
    private readonly IConsumer<string, string> _consumer;

    public ConsumerWrapper(ILogger<ConsumerWrapper> logger,
                           ConsumerConfig config,
                           string topicName)
    {
        _logger = logger;
        _consumer = new ConsumerBuilder<string, string>(config)
          .SetErrorHandler((_, e) => _logger.LogError(e.Reason))
          .Build();

        _consumer.Subscribe(topicName);
    }

    public string ReadMessages(CancellationToken cancellationToken)
    {
        try
        {
            ConsumeResult<string, string> result = new();

            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    result = _consumer.Consume(cancellationToken);
                }
                catch (ConsumeException exc)
                {
                    _logger.LogError(exc.Error.Reason);
                    continue;
                }

            }

            return result.Message.Value;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}
