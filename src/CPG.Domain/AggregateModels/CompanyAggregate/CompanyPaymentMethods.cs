using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class CompanyPaymentMethods : AuditableEntity<long>
{
    public CompanyPaymentMethods()
    {

    }

    public CompanyPaymentMethods(short methodType, long companyId)
    {
        MethodType = methodType;
        CompanyId = companyId;
    }

    public CompanyPaymentMethods(short methodType)
    {
        MethodType = methodType;
    }
    public static List<CompanyPaymentMethods> Create(short[] methodTypes)
    {
        if (methodTypes is null || !methodTypes.Any())
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new ArgumentException("detail is duplicated");

        var companyPaymentMethods = methodTypes.Select(i => new CompanyPaymentMethods(i)).ToList();
        return companyPaymentMethods;
    }

    public short MethodType { get; set; }
    public long CompanyId { get; set; }
}
