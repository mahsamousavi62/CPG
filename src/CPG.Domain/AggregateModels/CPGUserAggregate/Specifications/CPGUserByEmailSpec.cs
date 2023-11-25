using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Specifications
{
    public sealed class CPGUserByEmailSpec : Specification<CPGUser>, ISingleResultSpecification<CPGUser>
    {
        public CPGUserByEmailSpec(string email)
        {
            Query
                .Where(user => user.Email.Value == email);
        }
    }
}