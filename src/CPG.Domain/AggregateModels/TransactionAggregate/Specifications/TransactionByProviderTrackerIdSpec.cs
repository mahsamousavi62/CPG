using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByProviderTrackerIdSpec : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByProviderTrackerIdSpec(string providerTrackerId)
    {
        Query.Include(t => t.DirectDebitTransaction)
            .Include(t => t.PaymentRequest)
            .Where(a => a.DirectDebitTransaction.ProviderTrackerId == providerTrackerId && a.PaymentRequest.IsActive);
    }
}