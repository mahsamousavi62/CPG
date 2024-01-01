using MediatR;
using System;

namespace CPG.Domain.AggregateModels.BankAggregate.Events;

public class ChangeBankStatusEvent(int bankId, bool isActive, DateTime dateTime) : INotification
{
    public int BankId { get; } = bankId;

    public DateTime ModificationDateTime { get; set; } = dateTime;

    public bool IsActive { get; set; } = isActive;
}
