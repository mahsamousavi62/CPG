using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public class CompanyByPersianNameUpdateMode : Specification<Company>, ISingleResultSpecification<Company>
    {
        public CompanyByPersianNameUpdateMode(string persianName,long id)
        {
            Query.Where(c => c.PersianName == persianName && c.Id!=id);
        }
    }
}
