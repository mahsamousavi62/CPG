using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Events;
using CPG.Domain.AggregateModels.ProviderAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.ProviderAggregate;

public class Provider : AuditableEntity<long>, IAggregateRoot
{
    public Provider()
    {

    }
    public Provider(PersianName persianName, EnglishName englishName, ProviderType providerType, Logo logo, string providerData)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        ProviderType = providerType;
        Logo = logo.Value;
        ProviderData = providerData;
    }
 
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public ProviderType ProviderType { get; set; }
    public string Logo { get; set; }
    public string ProviderData { get; set; }
    public List<ProviderPaymentMethod> PaymentMethods { get; set; } = [];
    public static Provider Create(PersianName persianName, EnglishName englishName, ProviderType providerType, 
        Logo logo, string providerData, Enums.PaymentMethodType[] details)
    {
        var provider = new Provider(persianName, englishName, providerType, logo, providerData);
        provider.IsActive = true;
        var paymentMethods = ProviderPaymentMethod.Create(details);
        provider.PaymentMethods.AddRange(paymentMethods);
        return provider;
    }

    public static void Update(Provider provider,PersianName persianName, EnglishName englishName, ProviderType providerType,
        Logo logo, string providerData, Enums.PaymentMethodType[] details)
    {
        provider.PersianName = persianName.Value;
        provider.EnglishName = englishName.Value;
        provider.ProviderType  = providerType;
        provider.ProviderData = providerData; 
        provider.Logo = logo.Value;

        foreach (var newItem in details)
        {
            if (!provider.PaymentMethods.Any(p => p.MethodType == newItem))
                provider.PaymentMethods.Add(ProviderPaymentMethod.Create(newItem));
        }

        foreach (var currnetItem in provider.PaymentMethods)
        {
            if (!details.Any(p => p == currnetItem.MethodType))
                currnetItem.IsActive = false;
        }
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
