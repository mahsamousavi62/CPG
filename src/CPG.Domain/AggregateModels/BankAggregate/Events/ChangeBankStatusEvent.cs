using MediatR;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate.Events;

public class ChangeBankStatusEvent(int bankId, BankStatus status, long userId, DateTime dateTime) : INotification
{
    public int BankId { get; } = bankId;

    public long UserId { get; set; } = userId;

    public DateTime ModificationDateTime { get; set; } = dateTime;

    public BankStatus Status { get; set; } = status;
}
