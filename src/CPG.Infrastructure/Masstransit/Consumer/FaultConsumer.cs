using MassTransit;
using CPG.Domain.SharedKernel.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace CPG.Infrastructure.Masstransit.Consumer;

internal sealed class FaultConsumer : IConsumer<Fault>
{
    private readonly ILogService _logService;

    public FaultConsumer(ILogService logService)
    {
        _logService = logService;
    }

    public Task Consume(ConsumeContext<Fault> context)
    {
        var exceptions = JsonSerializer.Serialize(context.Message.Exceptions, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        var callLog = new CallLogModel
        {
            RequestBody = $"MessageId: {context.MessageId}, CorrelationId: {context.CorrelationId}, InitiatorId: {context.InitiatorId}",
            ResponseBody = exceptions,
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = "MassTransit Fault Consumer",
            ServiceCallStatus = false,
            ServiceType = Enums.ServiceType.MassTransit,
            CreationDate = DateTime.Now,
            CreationUserId = 1,
            ErrorCode = "FaultMessage",
            ErrorType = "Message consuming made a fault",
            ProviderType = Enums.ProviderTypeInLog.MassTransit,
            AuditType = Enums.AuditType.Develop
        };
        _logService.LogError(callLog);

        return Task.CompletedTask;
    }
}