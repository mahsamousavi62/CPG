using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Events
{
    public class NewCompanyDepositCreatedEvent : INotification
    {
        public long CompanyDepositId { get; }
        public DateTime DateOccurred { get; }

        public NewCompanyDepositCreatedEvent(long companyDepositId, DateTime dateOccurred)
        {
            CompanyDepositId = companyDepositId;
            DateOccurred = dateOccurred;
        }
    }
}
