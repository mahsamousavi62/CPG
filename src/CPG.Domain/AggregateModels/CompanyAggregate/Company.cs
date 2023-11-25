using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Company(PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo) : AuditableEntity<long>, IAggregateRoot
{
    private readonly string _persianName = persianName.Value;

    private readonly string _englishName = englishName.Value;

    private readonly string _logo = logo.Value;

    private readonly bool _nationalCodeMatchingRequied = nationalCodeMatchingRequied;

    public string PersianName => _persianName;

    public string EnglishName => _englishName;

    public string Logo => _logo;

    public bool NationalCodeMatchingRequied => _nationalCodeMatchingRequied;

    public List<CompanyPaymentMethods> PaymentMethods { get; set; } = [];

    public static Company Create(PersianName persianName, EnglishName englishName,
        bool nationalCodeMatchingRequied, Logo logo, short[] details)
    {
        var comapny = new Company(persianName, englishName, nationalCodeMatchingRequied, logo);
        var companyPaymentMethods = CompanyPaymentMethods.Create(details);
        comapny.PaymentMethods.AddRange(companyPaymentMethods);

        comapny.AddDomainEvent(new NewCompanyCreatedEvent(comapny.Id, DateTime.UtcNow));

        return comapny;
    }
}
