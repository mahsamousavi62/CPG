using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByDDGrantIdSpec : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByDDGrantIdSpec(long grantId)
    {
        Query.Include(t => t.DirectDebitTransaction)
            .Include(t => t.PaymentRequest)
            .Where(a => a.DirectDebitTransaction.DirectDebitGrantId == grantId && a.PaymentRequest.IsActive);
    }
}