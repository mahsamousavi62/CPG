using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByIPGTrackId : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByIPGTrackId(string trackId)
    {
        Query.Include(t => t.IPGTransaction)
             .Where(c => c.IPGTransaction.TrackId == trackId);
    }
}