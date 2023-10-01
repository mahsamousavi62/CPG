using System;
using Ardalis.Specification;
using CPG.Domain.AggregateModels.CPGUserAggregate;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Specifications
{
    public sealed class CPGUserByEmailSpec : Specification<CPGUser>, ISingleResultSpecification
    {
        public CPGUserByEmailSpec(string email)
        {
            Query
                .Where(user => user.Email.Value == email);
        }
    }
}