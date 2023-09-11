using System;
using Ardalis.Specification;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications
{
    public sealed class DaryaftyarUserByEmailSpec : Specification<DaryaftyarUser>, ISingleResultSpecification
    {
        public DaryaftyarUserByEmailSpec(string email)
        {
            Query
                .Where(user => user.Email.Value == email);
        }
    }
}