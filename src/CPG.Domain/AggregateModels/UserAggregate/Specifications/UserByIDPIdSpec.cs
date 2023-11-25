using Ardalis.Specification;
using System.Linq;

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
