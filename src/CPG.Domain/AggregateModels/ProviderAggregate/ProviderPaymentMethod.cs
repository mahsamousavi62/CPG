

using System;
using System.Collections.Generic;
using System.Linq;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.ProviderAggregate;

public class ProviderPaymentMethod : AuditableEntity<long>
{
    public byte MethodType { get; set; }
    public long ProviderId { get; set; }
    public Provider Provider { get; set; }

    public ProviderPaymentMethod(byte methodType, long providerId)
    {
        MethodType = methodType;
        ProviderId = providerId;
        IsActive = true;
    }

    public ProviderPaymentMethod(byte methodType)
    {
        MethodType = methodType;
    }

    public static List<ProviderPaymentMethod> Create(byte[] methodTypes)
    {
        if (methodTypes is null || methodTypes.Length == 0)
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        if (!methodTypes.All(methodType => Enum.IsDefined(typeof(Enums.PaymentMethodType), methodType)))
            throw new InvalidPaymentMethodType(nameof(methodTypes));

        var providerPaymentMethods = methodTypes.Select(i => new ProviderPaymentMethod(i)).ToList();
        return providerPaymentMethods;
    }
}
