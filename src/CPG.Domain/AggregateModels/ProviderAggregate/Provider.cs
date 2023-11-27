using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.ProviderAggregate;

public class Provider : AuditableEntity<long>, IAggregateRoot
{
    public Provider()
    {

    }
    public Provider(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData)
    {
        _persianName = persianName.Value;
        _englishName = englishName.Value;
        _providerType = providerType;
        _logo = logo.Value;
        _providerData = providerData;
    }

    private string _persianName;
    private string _englishName;
    private ProviderType _providerType;
    private string _logo;
    private string _providerData;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public ProviderType ProviderType => _providerType;
    public string Logo => _logo;
    public string ProviderData => _providerData;

    public static Provider Create(PersianName persianName, EnglishName englishName,
       ProviderType providerType, Logo logo, string providerData)
    {
        var provider = new Provider(persianName, englishName, providerType, logo, providerData);

        return provider;
    }
}
