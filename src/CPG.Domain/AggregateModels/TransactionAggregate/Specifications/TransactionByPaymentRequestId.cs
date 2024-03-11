using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class TransactionByPaymentRequestId : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public TransactionByPaymentRequestId(long paymentRequestId)
    {
        Query.Where(c => c.PaymentRquestId == paymentRequestId)
            .Include(t => t.DestinationDeposit)
            .Include(t => t.IPGTransaction)
            .ThenInclude(t => t.CompanyIPG)
            .ThenInclude(t => t.Provider)
            .Include(t => t.DirectDebitTransaction)
            .Include(t => t.PaymentReceiptTransaction);
    }
}
