using Ardalis.Specification;
using System;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByUserAndBankSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByUserAndBankSpec(int bankId, long userId, long providerId)
    {
        Query.Where(t => t.BankId == bankId && t.UserId == userId && t.RevokeDateTime == null && t.ExpirationDate > DateTime.Now
            && t.Status == 2 && t.ProviderId == providerId);
    }
}
