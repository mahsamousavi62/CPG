using MediatR;
using System;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Events;

public class ChangeProviderStatusEvent(long providerId, bool isActive, DateTime dateTime) : INotification
{
    public long ProviderId { get; } = providerId;

    public DateTime ModificationDateTime { get; set; } = dateTime;

    public bool IsActive { get; set; } = isActive;
}
