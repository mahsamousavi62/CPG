using MediatR;
using System;

namespace CPG.Domain.AggregateModels.BankAggregate.Events;

public class DeleteBankEvent(int bankId, long userId, DateTime dateTime) : INotification
{
    public int BankId { get; } = bankId;

    public long UserId { get; set; } = userId;

    public DateTime ModificationDateTime { get; set; } = dateTime;
}

