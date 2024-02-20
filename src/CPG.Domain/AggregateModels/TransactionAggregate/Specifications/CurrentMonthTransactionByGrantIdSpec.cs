using Ardalis.Specification;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate.Specifications;

public class CurrentMonthTransactionByGrantIdSpec : Specification<Transaction>, ISingleResultSpecification<Transaction>
{
    public CurrentMonthTransactionByGrantIdSpec(long grantId)
    {
        Query.Include(t => t.DirectDebitTransaction)
            .Where(t => t.CreationDate.Date >= DateTime.Now.AddMonths(-1).Date && grantId == t.DirectDebitTransaction.DirectDebitGrantId &&
                       (t.Status == TransactionStatus.TransactionSucceeded || t.Status == TransactionStatus.SettlementSucceeded));
    }
}