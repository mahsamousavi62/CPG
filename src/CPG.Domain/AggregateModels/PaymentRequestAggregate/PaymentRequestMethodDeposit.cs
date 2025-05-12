using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;

public class PaymentRequestMethodDeposit : AuditableEntity<long>
{
    public long PaymentRequestMethodId { get; set; }

    public long CompanyDepositId { get; set; }

    public PaymentRequestMethod PaymentRequestMethod { get; set; }

    public CompanyDeposit CompanyDeposit { get; set; }


    public PaymentRequestMethodDeposit(long paymentRequestMethodId, long companyDepositId)
    {
        PaymentRequestMethodId = paymentRequestMethodId;
        CompanyDepositId = companyDepositId;
        IsActive = true;
    }

    public PaymentRequestMethodDeposit(long companyDepositId)
    {
        CompanyDepositId = companyDepositId;
        IsActive = true;
    }

    public static List<PaymentRequestMethodDeposit> Create(long[] companyDepositIds)
    {
        if (companyDepositIds is null || companyDepositIds.Length == 0)
            throw new ArgumentNullException(nameof(companyDepositIds));

        if (companyDepositIds.Select(x => x).Distinct().Count() != companyDepositIds.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(companyDepositIds));

        var paymentRequestMethodDeposits = companyDepositIds.Select(i => new PaymentRequestMethodDeposit(i)).ToList();
        return paymentRequestMethodDeposits;
    }

    internal static PaymentRequestMethodDeposit Create(long companyDepositId)
    {
        return new PaymentRequestMethodDeposit(companyDepositId);
    }
}