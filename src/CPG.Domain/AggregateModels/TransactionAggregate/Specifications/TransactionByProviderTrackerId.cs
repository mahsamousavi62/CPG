using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByProviderTrackerId : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByProviderTrackerId(string providerTrackerId)
    {
        Query.Include(t => t.DestinationDeposit)
            .Include(t => t.IPGTransaction)
            .Where(t => t.IPGTransaction.IsActive && t.IPGTransaction.ProviderTrackerId == providerTrackerId);
    }
}
