using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public class CompanyByPersianName : Specification<Company>, ISingleResultSpecification<Company>
    {
        public CompanyByPersianName(string persianName)
        {
            Query.Where(c => c.PersianName == persianName);
        }
    }
}
