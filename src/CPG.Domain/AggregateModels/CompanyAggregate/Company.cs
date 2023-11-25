using CPG.Domain.SeedWork;
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
        PaymentMethods = [];
    }

    private string _persianName;
    private string _englishName;
    private string _logo;
    private bool _nationalCodeMatchingRequied;

    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public string Logo => _logo;
    public bool NationalCodeMatchingRequied=>_nationalCodeMatchingRequied;
    public List<CompanyPaymentMethods>  PaymentMethods { get; set; }

    public static Company Create(PersianName persianName, EnglishName englishName, 
        bool nationalCodeMatchingRequied, Logo logo, short[] details)
    {
        var comapny = new Company(persianName, englishName, nationalCodeMatchingRequied, logo);
        var companyPaymentMethods = CompanyPaymentMethods.Create(details);
        comapny.PaymentMethods.AddRange(companyPaymentMethods);
        return comapny;
    }
}
