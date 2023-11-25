using MediatR;
using System;

namespace CPG.Domain.AggregateModels.BankAggregate.Events
{
    public class DeleteBankEvent : INotification
    {
        public int BankId { get; }

        public long UserId { get; set; }

        public DateTime ModificationDateTime { get; set; }

        public DeleteBankEvent(int bankId, long userId, DateTime dateTime)
        {
            BankId = bankId;
            UserId = userId;
            ModificationDateTime = dateTime;
        }
    }
}

