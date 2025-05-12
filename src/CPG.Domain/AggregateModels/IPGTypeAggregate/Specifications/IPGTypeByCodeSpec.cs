using Ardalis.Specification;
using System;
using System.Linq;

namespace CPG.Domain.AggregateModels.IPGTypeAggregate.Specifications;

public class IPGTypeByCodeSpec : Specification<IPGType>
{
    public IPGTypeByCodeSpec(short[] codes)
    {
        Query.Where(t => codes.Contains(t.Code));
    }
}
