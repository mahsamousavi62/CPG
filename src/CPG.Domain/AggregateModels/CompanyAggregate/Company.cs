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
        _persianName = persianName.Value;
        _englishName = englishName.Value;
        _nationalCodeMatchingRequied = nationalCodeMatchingRequied;
        _logo = logo.Value;
        PaymentMethods = new List<CompanyPaymentMethod>();
    }

    private string _persianName;
    private string _englishName;
    private string _logo;
    private bool _nationalCodeMatchingRequied;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public string Logo => _logo;
    public bool NationalCodeMatchingRequied => _nationalCodeMatchingRequied;
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
