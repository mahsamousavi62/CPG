using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Company : AuditableEntity<long>, IAggregateRoot
{
    public Company()
    {

    }
    public Company(PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo,
        Url siteAddress, Enums.IpgRedirectionMethodType ipgRedirectionMethodType, short code)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        NationalCodeMatchingRequied = nationalCodeMatchingRequied;
        Logo = logo.Value;
        SiteAddress = siteAddress.Value;
        IpgRedirectionMethodType = ipgRedirectionMethodType;
        PaymentMethods = [];
        Code = code;
        IsActive = true;
    }

    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public string Logo { get; set; }
    public bool NationalCodeMatchingRequied { get; set; }
    public string SiteAddress { get; set; }
    public IpgRedirectionMethodType IpgRedirectionMethodType { get; set; }
    public short? Code { get; set; }
    public List<CompanyDeposit> CompanyDeposits { get; set; }
    public List<CompanyPaymentMethod> PaymentMethods { get; set; } = [];
    public List<User> Users { get; set; }
    public List<PaymentRequest> PaymentRequests { get; set; }
    public List<CompanyIPG> CompanyIPGs { get; set; }
    public CompanyShaparakSetting ShaparakSetting { get; set; }

    public static Company Create(PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo,
        PaymentMethodType[] details, Url siteAddress, IpgRedirectionMethodType ipgRedirectionMethodType, string key, string iv,
        int? thirdPartyCode, short code)
    {
        var company = new Company(persianName, englishName, nationalCodeMatchingRequied, logo, siteAddress, ipgRedirectionMethodType, code);

        var companyPaymentMethods = CompanyPaymentMethod.Create(details);
        company.PaymentMethods.AddRange(companyPaymentMethods);

        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(iv) && thirdPartyCode != null)
        {
            var companyShaparakSetting = CompanyShaparakSetting.Create(key, iv, thirdPartyCode);
            company.ShaparakSetting = companyShaparakSetting;
        }

        company.AddDomainEvent(new NewCompanyCreatedEvent(company.Id, DateTime.Now));
        return company;
    }

    public static void Update(Company company, PersianName persianName, EnglishName englishName, bool nationalCodeMatchingRequied, Logo logo,
        PaymentMethodType[] methodTypes, Url siteAddress, IpgRedirectionMethodType ipgRedirectionMethodType, string key, string iV,
        int? thirdPartyCode, short code)
    {
        company.PersianName = persianName.Value;
        company.EnglishName = englishName.Value;
        company.Logo = logo.Value;
        company.NationalCodeMatchingRequied = nationalCodeMatchingRequied;
        company.SiteAddress = siteAddress.Value;
        company.IpgRedirectionMethodType = ipgRedirectionMethodType;
        company.Code = code;

        foreach (var newItem in methodTypes)
        {
            if (!company.PaymentMethods.Any(p => p.MethodType == newItem))
                company.PaymentMethods.Add(CompanyPaymentMethod.Create(newItem));
        }

        foreach (var currnetItem in company.PaymentMethods.ToList())
        {
            if (!methodTypes.Any(p => p == currnetItem.MethodType))
                company.PaymentMethods.Remove(currnetItem);
        }

        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(iV) && thirdPartyCode != null)
        {
            if (company.ShaparakSetting is null)
                company.ShaparakSetting = CompanyShaparakSetting.Create(key, iV, thirdPartyCode);

            else
                CompanyShaparakSetting.Update(company.ShaparakSetting, key, iV, thirdPartyCode);
        }
        else
            company.ShaparakSetting = null;
    }
}