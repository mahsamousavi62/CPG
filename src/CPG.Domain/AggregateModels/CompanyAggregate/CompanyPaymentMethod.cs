using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class CompanyPaymentMethod : AuditableEntity<long>
{
    public CompanyPaymentMethod(short methodType, long companyId)
    {
        MethodType = methodType;
        CompanyId = companyId;
    }

    public CompanyPaymentMethod(short methodType)
    {
        MethodType = methodType;
    }
    public static List<CompanyPaymentMethod> Create(short[] methodTypes)
    {
        if (methodTypes is null || !methodTypes.Any())
            throw new ArgumentNullException(nameof(methodTypes));

        if (methodTypes.Select(x => x).Distinct().Count() != methodTypes.Length)
            throw new DuplicatePaymentMethodTypeException(nameof(methodTypes));

        if (!methodTypes.All(methodType => Enum.IsDefined(typeof(Enums.CompanyPaymentMethodType), methodType)))
            throw new InvalidPaymentMethodType(nameof(methodTypes));

        var companyPaymentMethods = methodTypes.Select(i => new CompanyPaymentMethod(i)).ToList();
        return companyPaymentMethods;
    }

    public short MethodType { get; set; }
    public long CompanyId { get; set; }
    public Company Company { get; set; }
}
