using CPG.Domain.AggregateModels.ProviderAggregate.Events;
using CPG.Domain.AggregateModels.ProviderAggregate.Exceptions;
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
    public Provider(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData, short verificationTimeLimit, Url ipgBaseUrl)
    {
        _persianName = persianName.Value;
        _englishName = englishName.Value;
        _providerType = providerType;
        _logo = logo.Value;
        _providerData = providerData;
        _ipgVerificationTimeLimit = verificationTimeLimit;
        _ipgBaseUrl = ipgBaseUrl.Value;
    }

    private string _persianName;
    private string _englishName;
    private ProviderType _providerType;
    private string _logo;
    private string _providerData;
    private short _ipgVerificationTimeLimit;
    public string _ipgBaseUrl;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public ProviderType ProviderType => _providerType;
    public string Logo => _logo;
    public string ProviderData => _providerData;
    public short IpgVerificationTimeLimit => _ipgVerificationTimeLimit;
    public string IpgBaseUrl => _ipgBaseUrl;

    public static Provider Create(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData, short verificationTimeLimit, Url ipgBaseUrl)
    {
        var provider = new Provider(persianName, englishName, providerType, logo, providerData, verificationTimeLimit, ipgBaseUrl);
        provider.IsActive = true;
        return provider;
    }

    public void Update(string persianName, string englishName, ProviderType providerType, string providerData, Logo logo, short verificationTimeLimit, Url ipgBaseUrl)
    {
        _persianName = persianName;
        _englishName = englishName;
        _providerType = providerType;
        _providerData = providerData;
        _logo = logo.Value;
        _ipgVerificationTimeLimit = verificationTimeLimit;
        _ipgBaseUrl = ipgBaseUrl;
    }

    public void SetAsActive(long userId)
    {
        if (IsActive == true)
            throw new ProviderIsActiveException(Id);

        IsActive = true;

        AddDomainEvent(new ChangeProviderStatusEvent(Id, IsActive, DateTime.Now));
    }

    public void SetAsInactive(long userId)
    {
        if (IsActive == false)
            throw new ProviderIsNotActiveException(Id);

        IsActive = false;

        AddDomainEvent(new ChangeProviderStatusEvent(Id, IsActive, DateTime.Now));
    }
}
