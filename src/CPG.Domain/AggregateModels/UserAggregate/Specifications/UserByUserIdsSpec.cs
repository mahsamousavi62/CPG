using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace CPG.Domain.AggregateModels.UserAggregate.Specifications
{
    public class UserByUserIdsSpec : Specification<User>
    {
        public UserByUserIdsSpec(List<long> userIds)
        {
            Query.Where(entity => userIds.Contains(entity.Id));
        }
    }
}
