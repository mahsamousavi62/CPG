using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public class CompanyByEnglishNameUpdateMode: Specification<Company>, ISingleResultSpecification<Company>
    {
        public CompanyByEnglishNameUpdateMode(string englishName,long id)
        {
            Query.Where(c=>c.EnglishName== englishName && c.Id!=id);
        }
    }
}
