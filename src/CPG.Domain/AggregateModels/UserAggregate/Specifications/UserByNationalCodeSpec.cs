using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.UserAggregate.Specifications;
    public class UserByNationalCodeSpec : Specification<User>, ISingleResultSpecification<User>
    {
        public UserByNationalCodeSpec(string nationalCode)
        {
        Query.Where(u=>u.NationalCode==nationalCode);
        }
    }

