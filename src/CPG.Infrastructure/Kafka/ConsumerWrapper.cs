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
        var callLog = new CallLogModel
        {
            RequestBody = "",
            ResponseBody = errorReason,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = "Kafka Consumer",
            ServiceCallStatus = false,
            ServiceType = Enums.ServiceType.Kafka,
            CreationDate = DateTime.Now,
            CreationUserId = 1,
            ErrorCode = "KafkaError",
            ErrorType = errorReason,
            ProviderType = Enums.ProviderTypeInLog.Kafka,
            AuditType = Enums.AuditType.Develop
        };
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
                    var callLog = new CallLogModel
                    {
                        RequestBody = "",
                        ResponseBody = exc.Error.Reason,
                        ServiceCallDate = DateTime.Now,
                        ServiceCallUrl = "Kafka Consumer",
                        ServiceCallStatus = false,
                        ServiceType = Enums.ServiceType.Kafka,
                        CreationDate = DateTime.Now,
                        CreationUserId = 1,
                        ErrorCode = exc.Error.Code.ToString(),
                        ErrorType = exc.Error.Reason,
                        ProviderType = Enums.ProviderTypeInLog.Kafka,
                        AuditType = Enums.AuditType.Develop
                    };
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
