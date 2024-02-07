using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByTrackIdSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByTrackIdSpec(string trackId)
    {
        Query.Where(t => t.TrackId == trackId)
            .Include(t => t.Provider);
    }
}