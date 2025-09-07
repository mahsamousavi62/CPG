using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByCharismaCardTrackId : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
	public TransactionByCharismaCardTrackId(string trackId)
	{
		Query.Include(t => t.CharismaCardTransaction)
			 .Where(c => c.CharismaCardTransaction.TrackId == trackId);
	}
}