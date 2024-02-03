using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
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
    private short _ipgVerificationTimeLimit;
    public string _ipgBaseUrl;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public ProviderType ProviderType => _providerType;
    public string Logo => _logo;
    public string ProviderData => _providerData;
    public List<ProviderPaymentMethod> PaymentMethods { get; set; } = [];
    public List<DirectDebitGrant> DirectDebitGrants { get; set; }

    public static Provider Create(PersianName persianName, EnglishName englishName, ProviderType providerType, 
        Logo logo, string providerData, Enums.PaymentMethodType[] details)
    {
        var provider = new Provider(persianName, englishName, providerType, logo, providerData);
        provider.IsActive = true;
        var paymentMethods = ProviderPaymentMethod.Create(details);
        provider.PaymentMethods.AddRange(paymentMethods);
        return provider;
    }

    public void Update(string persianName, string englishName, ProviderType providerType, string providerData, Logo logo)
    {
        _persianName = persianName;
        _englishName = englishName;
        _providerType = providerType;
        _providerData = providerData;
        _logo = logo.Value;
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
