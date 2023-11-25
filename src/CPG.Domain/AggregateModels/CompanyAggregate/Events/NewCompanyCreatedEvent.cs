using System;
using MediatR;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Events
{
    public class NewCompanyCreatedEvent : INotification
    {
        public long CompanyId { get; }
        public DateTime DateOccurred { get; }

        public NewCompanyCreatedEvent(long companyId, DateTime dateOccurred)
        {
            CompanyId = companyId;
            DateOccurred = dateOccurred;
        }
    }
}
