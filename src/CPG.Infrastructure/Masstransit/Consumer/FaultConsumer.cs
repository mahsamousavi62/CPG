using MassTransit;
using CPG.Domain.SharedKernel;
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

        var callLog = CallLogModel.CreateError(
            serviceName: "MassTransitFaultConsumer",
            providerName: "MassTransit",
            requestUri: "MassTransit Fault Consumer",
            requestBody: $"MessageId: {context.MessageId}, CorrelationId: {context.CorrelationId}, InitiatorId: {context.InitiatorId}",
            responseBody: exceptions,
            exception: null,
            serviceType: Enums.ServiceType.MassTransit,
            providerType: Enums.ProviderTypeInLog.MassTransit,
            auditType: Enums.AuditType.Develop,
            correlationId: context.CorrelationId?.ToString(),
            userId: 1
        );
        _logService.LogError(callLog);

        return Task.CompletedTask;
    }
}