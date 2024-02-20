using Ardalis.Specification;
using CPG.Domain.SharedKernel;
using System;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByUserSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByUserSpec(long userId, decimal amount, bool nationalCodeMatchingRequied)
    {
        Query.Where(t => t.UserId == userId && t.RevokeDateTime == null && t.ExpirationDate > DateTime.Now && 
                t.Status == Enums.DirectDebitGrantStatus.Activated && t.Bank.IsActive && t.Bank.HasDirectDebitFeature == true &&
                t.Bank.DirectDebitSetting.IsActive && t.Bank.DirectDebitSetting.Provider.IsActive &&
                t.Bank.DirectDebitSetting.Provider.PaymentMethods.Any(p => p.MethodType == Enums.PaymentMethodType.DirectDebit && p.IsActive) &&
                t.Bank.DirectDebitSetting.MaxWithdrawalAmountPerDay >= amount && (nationalCodeMatchingRequied == true ?
                t.Bank.DirectDebitSetting.AuthenticationType == Enums.AuthenticationType.CheckMobileAndDepositOwnershipMatching : true))
            .Include(t => t.Bank)
            .ThenInclude(t => t.DirectDebitSetting)
            .ThenInclude(t => t.Provider)
            .ThenInclude(t => t.PaymentMethods);
    }
}