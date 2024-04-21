using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate;

public class CompanyDepositPaymentMethod : AuditableEntity<long>
{
    public Enums.PaymentMethodType MethodType { get; set; }
    public long CompanyDepositId { get; set; }
    public CompanyDeposit CompanyDeposit { get; set; }

    public CompanyDepositPaymentMethod(Enums.PaymentMethodType methodType, long companyDepositId)
    {
        MethodType = methodType;
        CompanyDepositId = companyDepositId;
        IsActive = true;
    }

    public CompanyDepositPaymentMethod(Enums.PaymentMethodType methodType)
    {
        MethodType = methodType;
        IsActive = true;
    }

    public static List<CompanyDepositPaymentMethod> Create(Enums.PaymentMethodType[] methodTypes)
    {
        if (methodTypes is null || methodTypes.Length == 0)
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        var companyDepositPaymentMethods = methodTypes.Select(i => new CompanyDepositPaymentMethod(i)).ToList();
        return companyDepositPaymentMethods;
    }

    public static CompanyDepositPaymentMethod Create(Enums.PaymentMethodType newItem)
    {
        return new CompanyDepositPaymentMethod(newItem);
    }
}