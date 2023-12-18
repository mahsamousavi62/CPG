using Ardalis.Specification;
using CPG.Application.UseCases.CompanyDeposits;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;

public class CompanyDepositsByIdList : Specification<CompanyDeposit>
{
    public CompanyDepositsByIdList(List<long> idList)
    {
        Query.Where(c => idList.Contains(c.Id));
    }
}
