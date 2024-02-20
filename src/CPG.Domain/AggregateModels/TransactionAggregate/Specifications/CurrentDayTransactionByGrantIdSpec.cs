using Ardalis.Specification;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class CurrentDayTransactionByGrantIdSpec : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public CurrentDayTransactionByGrantIdSpec(long grantId)
    {
        Query.Include(t => t.DirectDebitTransaction)            
            .Where(t => t.CreationDate.Date == DateTime.Now.Date && grantId == t.DirectDebitTransaction.DirectDebitGrantId &&
                       (t.Status == TransactionStatus.TransactionSucceeded || t.Status == TransactionStatus.SettlementSucceeded));
    }
}