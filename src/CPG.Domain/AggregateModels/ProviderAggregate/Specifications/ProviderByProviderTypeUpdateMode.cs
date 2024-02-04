using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications
{
    public class ProviderByProviderTypeUpdateMode : Specification<Provider>, ISingleResultSpecification<Provider>
    {
        public ProviderByProviderTypeUpdateMode(Enums.ProviderType providerType,long id)
        {
            Query.Where(c => c.ProviderType==providerType && c.Id!=id);
        }
    }
}
