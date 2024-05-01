using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;

public class PaymentRequestMethod : AuditableEntity<long>
{
    public long PaymentRequestId { get; set; }

    public Enums.PaymentMethodType PaymentMethodType { get; set; }

    public PaymentRequest PaymentRequest { get; set; }

    public List<PaymentRequestMethodDeposit> PaymentRequestMethodDeposits { get; set; }

    public List<PaymentRequestMethodIpgType> PaymentRequestMethodIpgTypes { get; set; }

    public PaymentRequestMethod()
    {
        
    }

    public PaymentRequestMethod(Enums.PaymentMethodType methodType, long paymentRequestId)
    {
        PaymentMethodType = methodType;
        PaymentRequestId = paymentRequestId;
        IsActive = true;
    }

    public PaymentRequestMethod(Enums.PaymentMethodType methodType)
    {
        PaymentMethodType = methodType;
        IsActive = true;
    }

    public static List<PaymentRequestMethod> Create(Enums.PaymentMethodType[] methodTypes)
    {
        if (methodTypes is null || methodTypes.Length == 0)
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        var paymentMethods = methodTypes.Select(i => new PaymentRequestMethod(i)).ToList();
        return paymentMethods;
    }
}