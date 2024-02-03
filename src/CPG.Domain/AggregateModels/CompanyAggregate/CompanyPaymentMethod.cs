using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class CompanyPaymentMethod : AuditableEntity<long>
{
    public Enums.PaymentMethodType MethodType { get; set; }
    public long CompanyId { get; set; }
    public Company Company { get; set; }

    public CompanyPaymentMethod(Enums.PaymentMethodType methodType, long companyId)
    {
        MethodType = methodType;
        CompanyId = companyId;
        IsActive = true;
    }

    public CompanyPaymentMethod(Enums.PaymentMethodType methodType)
    {
        MethodType = methodType;
        IsActive = true;
    }

    public static List<CompanyPaymentMethod> Create(Enums.PaymentMethodType[] methodTypes)
    {
        if (methodTypes is null || methodTypes.Length == 0)
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        var companyPaymentMethods = methodTypes.Select(i => new CompanyPaymentMethod(i)).ToList();
        return companyPaymentMethods;
    }

    public static CompanyPaymentMethod Create(Enums.PaymentMethodType newItem)
    {
        return new CompanyPaymentMethod(newItem);
    }
}
