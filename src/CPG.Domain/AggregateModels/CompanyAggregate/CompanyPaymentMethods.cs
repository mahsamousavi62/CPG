using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class CompanyPaymentMethods : AuditableEntity<long>
{
   

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

        if (!methodTypes.All(methodType => Enum.IsDefined(typeof(Enums.CompanyPaymentMethodType), methodType)))
            throw new Exception("invalid_CompanyPaymentMethodType");

       var companyPaymentMethods = methodTypes.Select(i => new CompanyPaymentMethods(i)).ToList();
        return companyPaymentMethods;
    }

    public short MethodType { get; set; }
    public long CompanyId { get; set; }
    public Company Company { get; set; }
}
