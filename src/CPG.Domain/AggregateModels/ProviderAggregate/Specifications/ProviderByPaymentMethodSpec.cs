using Ardalis.Specification;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderByPaymentMethodSpec : Specification<Provider>
{
    public ProviderByPaymentMethodSpec(PaymentMethodType paymentMethodType)
    {
        Query.Include(c => c.PaymentMethods)
            .Where(c => c.PaymentMethods.Select(t => t.MethodType).Contains(paymentMethodType));
    }
}
