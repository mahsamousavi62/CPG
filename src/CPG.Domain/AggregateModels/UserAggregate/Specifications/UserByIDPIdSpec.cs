using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.UserAggregate.Specifications
{
    public class UserByIDPIdSpec : Specification<User>, ISingleResultSpecification<User>
    {
        public UserByIDPIdSpec(string iDPId)
        {
            Query.Where(user => user.IDPId == iDPId);
        }
    }
}
