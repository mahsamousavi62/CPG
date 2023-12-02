using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Events;
using CPG.Domain.AggregateModels.ProviderAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
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

    public static Provider Create(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData)
    {
        var provider = new Provider(persianName, englishName, providerType, logo, providerData);
        provider.CreationDate = DateTime.Now;
        return provider;
    }

    public void Update(string persianName, string englishName, ProviderType providerType, string providerData, Logo logo)
    {
        _persianName = persianName;
        _englishName = englishName;
        _providerType = providerType;
        _providerData = providerData;
        _logo = logo.Value;
        SetModificationData();
    }

    public void SetModificationData()
    {
        ModificationDate = DateTime.Now;
    }

    public void SetAsActive(long userId)
    {
        if (IsActive == true)
            throw new ProviderIsActiveException(Id);

        IsActive = true;
        SetModificationData();

        AddDomainEvent(new ChangeProviderStatusEvent(Id, IsActive, DateTime.Now));
    }

    public void SetAsInactive(long userId)
    {
        if (IsActive == false)
            throw new ProviderIsNotActiveException(Id);

        IsActive = false;
        SetModificationData();

        AddDomainEvent(new ChangeProviderStatusEvent(Id, IsActive, DateTime.Now));
    }
}
