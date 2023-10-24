using MediatR;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate.Events
{
    public class ChangeBankStatusEvent : INotification
    {
        public int BankId { get; }
        
        public long UserId { get; set; }

        public DateTime ModificationDateTime { get; set; }

        public BankStatus Status { get; set; }

        public ChangeBankStatusEvent(int bankId, BankStatus status, long userId, DateTime dateTime)
        {
            BankId = bankId;
            Status = status;
            UserId = userId;
            ModificationDateTime = dateTime;            
        }
    }
}
