using Ardalis.Specification;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitPlanByBankValidityDurationSpec : Specification<DirectDebitPlan>
{
    public DirectDebitPlanByBankValidityDurationSpec(short validityDurationPerMonth)
    {
        Query.Where(t => t.DurationPerMonth <= validityDurationPerMonth);
    }
}