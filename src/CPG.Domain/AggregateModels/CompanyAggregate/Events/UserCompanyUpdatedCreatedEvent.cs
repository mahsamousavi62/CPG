using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Events
{
    public class UserCompanyUpdatedCreatedEvent : INotification
    {
        public long CompanyId { get; }
        public List<long> UserIds { get; }

        public UserCompanyUpdatedCreatedEvent(long companyId, List<long> userIds)
        {
            CompanyId = companyId;
            UserIds = userIds;
        }

    }
}
