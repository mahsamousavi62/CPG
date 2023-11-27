using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public class CompanyByEnglishName: Specification<Company>, ISingleResultSpecification<Company>
    {
        public CompanyByEnglishName(string englishName)
        {
            Query.Where(c=>c.EnglishName== englishName);
        }
    }
}
