using MediatR;
using System;

namespace CPG.Domain.AggregateModels.BankAggregate.Events;

public class ChangeBankStatusEvent(int bankId, bool isActive, long userId, DateTime dateTime) : INotification
{
    public int BankId { get; } = bankId;

    public long UserId { get; set; } = userId;

    public DateTime ModificationDateTime { get; set; } = dateTime;

    public bool IsActive { get; set; } = isActive;
}
