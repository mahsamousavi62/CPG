using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByPersianNameUpdateMode : Specification<Application>, ISingleResultSpecification<Application>
    {
        public ApplicationByPersianNameUpdateMode(string persianName,int id)
        {
            Query.Where(c => c.PersianName == persianName && c.Id!=id);
        }
    }

