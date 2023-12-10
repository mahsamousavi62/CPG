using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Company : AuditableEntity<long>, IAggregateRoot
{
    public Company()
    {

    }

    public Company(PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        NationalCodeMatchingRequied = nationalCodeMatchingRequied;
        Logo = logo.Value;
        PaymentMethods = [];
        IsActive = true;
    }

    public string PersianName { get; }

    public string EnglishName { get; }

    public string Logo { get; }

    public bool NationalCodeMatchingRequied { get; }

    public List<CompanyDeposit> CompanyDeposits { get; set; }

    public List<CompanyPaymentMethod> PaymentMethods { get; set; } = [];

    public List<User> Users { get; set; }

    public static Company Create(PersianName persianName, EnglishName englishName,
        bool nationalCodeMatchingRequied, Logo logo, short[] details)
    {
        var comapny = new Company(persianName, englishName, nationalCodeMatchingRequied, logo);
        
        var companyPaymentMethods = CompanyPaymentMethod.Create(details);
        
        comapny.PaymentMethods.AddRange(companyPaymentMethods);

        comapny.AddDomainEvent(new NewCompanyCreatedEvent(comapny.Id, DateTime.UtcNow));

        return comapny;
    }
}
