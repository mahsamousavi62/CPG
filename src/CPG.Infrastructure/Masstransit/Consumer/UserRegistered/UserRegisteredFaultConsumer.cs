using Charisma.MessagingContracts.UsersManagement.User;
using MassTransit;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace CPG.Infrastructure.Masstransit.Consumer.UserRegistered
{
    internal sealed class UserRegisteredFaultConsumer : IConsumer<Fault<IUserRegistered>>
    {
        private readonly ILogService _logService;

        public UserRegisteredFaultConsumer(ILogService logService)
        {
            _logService = logService;
        }

        public Task Consume(ConsumeContext<Fault<IUserRegistered>> context)
        {
            var exceptions = JsonSerializer.Serialize(context.Message.Exceptions, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            var callLog = CallLogModel.CreateError(
                serviceName: "UserRegisteredFaultConsumer",
                providerName: "MassTransit",
                requestUri: "MassTransit UserRegistered Fault Consumer",
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
}
