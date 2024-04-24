using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications
{
    public class TransactionByPaymentReceiptId : Specification<Transaction>, ISingleResultSpecification<Transaction>
    {
        public TransactionByPaymentReceiptId(long paymentReceiptId)
        {
            Query.Include(t => t.PaymentReceiptTransaction)
           .Include(t=>t.PaymentRequest)
           .Where(c => c.PaymentReceiptTransactionId == paymentReceiptId);
        }
    }
}
