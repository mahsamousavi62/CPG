

using System;
using System.Collections.Generic;
using System.Linq;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.ProviderAggregate;

public class ProviderPaymentMethod : AuditableEntity<long>
{
    public Enums.PaymentMethodType MethodType { get; set; }
    public long ProviderId { get; set; }
    public Provider Provider { get; set; }

    public ProviderPaymentMethod(Enums.PaymentMethodType methodType, long providerId)
    {
        MethodType = methodType;
        ProviderId = providerId;
        IsActive = true;
    }

    public ProviderPaymentMethod(Enums.PaymentMethodType methodType)
    {
        MethodType = methodType;
        IsActive= true;
    }

    public static List<ProviderPaymentMethod> Create(Enums.PaymentMethodType[] methodTypes)
    {
        if (methodTypes is null || methodTypes.Length == 0)
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        var providerPaymentMethods = methodTypes.Select(i => new ProviderPaymentMethod(i)).ToList();
        return providerPaymentMethods;
    }
}
