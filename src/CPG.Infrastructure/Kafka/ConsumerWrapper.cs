using Confluent.Kafka;
using CPG.Domain.SharedKernel.Logging;
using System;
using System.Threading;

namespace CPG.Infrastructure.Kafka;

public class ConsumerWrapper
{
    private readonly ILogService _logService;
    private readonly IConsumer<string, string> _consumer;

    public ConsumerWrapper(ILogService logService,
                           ConsumerConfig config,
                           string topicName)
    {
        _logService = logService;
        _consumer = new ConsumerBuilder<string, string>(config)
          .SetErrorHandler((_, e) => LogKafkaError(e.Reason))
          .Build();

        _consumer.Subscribe(topicName);
    }

    private void LogKafkaError(string errorReason)
    {
        var callLog = CallLogModel.CreateError(
            serviceName: "KafkaConsumer",
            providerName: "Kafka",
            requestUri: "Kafka Consumer",
            requestBody: "",
            responseBody: errorReason,
            exception: null,
            serviceType: Enums.ServiceType.Kafka,
            providerType: Enums.ProviderTypeInLog.Kafka,
            auditType: Enums.AuditType.Develop,
            userId: 1
        );
        _logService.LogError(callLog);
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
                    var callLog = CallLogModel.CreateError(
                        serviceName: "KafkaConsumer",
                        providerName: "Kafka",
                        requestUri: "Kafka Consumer",
                        requestBody: "",
                        responseBody: exc.Error.Reason,
                        exception: exc,
                        serviceType: Enums.ServiceType.Kafka,
                        providerType: Enums.ProviderTypeInLog.Kafka,
                        auditType: Enums.AuditType.Develop,
                        userId: 1
                    );
                    _logService.LogError(callLog);
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
