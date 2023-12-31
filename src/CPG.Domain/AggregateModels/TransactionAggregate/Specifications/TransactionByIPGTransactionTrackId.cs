using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyAggregate;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByIPGTransactionTrackId : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByIPGTransactionTrackId(string trackId)
    {
        Query.Include(t => t.IPGTransaction).Include(t => t.PaymentRequest).
            Where(a => a.IPGTransaction.IsActive && a.IPGTransaction.TrackId == trackId && a.PaymentRequest.IsActive);
    }
}
