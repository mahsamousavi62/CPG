using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications
{
    public class ProviderByProviderType : Specification<Provider>, ISingleResultSpecification<Provider>
    {
        public ProviderByProviderType(Enums.ProviderType providerType)
        {
            Query.Where(c => c.ProviderType==providerType);
        }
    }
}
