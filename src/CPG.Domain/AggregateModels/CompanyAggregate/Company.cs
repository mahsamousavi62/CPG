using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
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

    public Company(PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo,
        Url siteAddress, Enums.IpgRedirectionMethodType ipgRedirectionMethodType)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        NationalCodeMatchingRequied = nationalCodeMatchingRequied;
        Logo = logo.Value;
        SiteAddress = siteAddress.Value;
        IpgRedirectionMethodType = ipgRedirectionMethodType;
        PaymentMethods = [];
        IsActive = true;
    }

    public string PersianName { get; }

    public string EnglishName { get; }

    public string Logo { get; }

    public bool NationalCodeMatchingRequied { get; }

    public string SiteAddress { get; set; }

    public Enums.IpgRedirectionMethodType IpgRedirectionMethodType { get; set; }

    public List<CompanyDeposit> CompanyDeposits { get; set; }

    public List<CompanyPaymentMethod> PaymentMethods { get; set; } = [];

    public List<User> Users { get; set; }

    public List<PaymentRequest> PaymentRequests { get; set; }

    public List<CompanyIPG> CompanyIPGs { get; set; }

    public CompanyShaparakSetting ShaparakSetting { get; set; }

    public static Company Create(PersianName persianName, EnglishName englishName,
        bool nationalCodeMatchingRequied, Logo logo, Enums.PaymentMethodType[] details, Url siteAddress,
        Enums.IpgRedirectionMethodType ipgRedirectionMethodType, string key, string iv, int? thirdPartyCode)
    {
        var company = new Company(persianName, englishName, nationalCodeMatchingRequied, logo, siteAddress, ipgRedirectionMethodType);

        var companyPaymentMethods = CompanyPaymentMethod.Create(details);        
        company.PaymentMethods.AddRange(companyPaymentMethods);

        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(iv) && thirdPartyCode != null)
        {
            var companyShaparakSetting = CompanyShaparakSetting.Create(key, iv, thirdPartyCode);
            company.ShaparakSetting = companyShaparakSetting;
        }

        company.AddDomainEvent(new NewCompanyCreatedEvent(company.Id, DateTime.UtcNow));
        return company;
    }
}
