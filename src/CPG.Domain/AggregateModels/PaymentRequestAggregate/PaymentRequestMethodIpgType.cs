using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;

public class PaymentRequestMethodIpgType : AuditableEntity<long>
{
    public long PaymentRequestMethodId { get; set; }

    public long IpgTypeId { get; set; }

    public PaymentRequestMethod PaymentRequestMethod { get; set; }

    public IPGType IPGType { get; set; }

    public PaymentRequestMethodIpgType(long paymentRequestMethodId, long ipgTypeId)
    {
        PaymentRequestMethodId = paymentRequestMethodId;
        IpgTypeId = ipgTypeId;
        IsActive = true;
    }

    public PaymentRequestMethodIpgType(long ipgTypeId)
    {
        IpgTypeId = ipgTypeId;
        IsActive = true;
    }

    public static List<PaymentRequestMethodIpgType> Create(long[] ipgTypeIds)
    {
        if (ipgTypeIds is null || ipgTypeIds.Length == 0)
            throw new ArgumentNullException(nameof(ipgTypeIds));

        if (ipgTypeIds.Select(x => x).Distinct().Count() != ipgTypeIds.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(ipgTypeIds));

        var paymentRequestMethodIpgTypes = ipgTypeIds.Select(i => new PaymentRequestMethodIpgType(i)).ToList();
        return paymentRequestMethodIpgTypes;
    }

    internal static PaymentRequestMethodIpgType Create(long ipgTypeId)
    {
        return new PaymentRequestMethodIpgType(ipgTypeId);
    }
}