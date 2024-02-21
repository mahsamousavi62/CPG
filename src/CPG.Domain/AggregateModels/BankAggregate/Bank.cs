using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class Bank : AuditableEntity<int>, IAggregateRoot
{
    internal string _name;
    internal string _logoAddress;
    internal IbanPrefix _ibanPrefix;
    internal bool? _hasDirectDebitFeature;

    public string Name => _name;
    public string LogoAddress => _logoAddress;
    public IbanPrefix IbanPrefix => _ibanPrefix;
    public bool? HasDirectDebitFeature => _hasDirectDebitFeature;
    public BankDirectDebitSetting DirectDebitSetting { get; set; }
    public List<CompanyDeposit> CompanyDeposits { get; set; }
    public List<DirectDebitGrant> DirectDebitGrants { get; set; }
    public Bank()
    {
    }

    public void Update(string name, Logo logo, IbanPrefix ibanPrefix, bool hasDirectDebitFeature, long? providerId, string ddBankCode, decimal? maxWithdrawalAmountPerDay,
        ValidityDuration? maxMandateValidityDurationPerMonth, AuthenticationType? authenticationType)
    {
        _ibanPrefix = ibanPrefix;
        _name = name;
        _logoAddress = logo.Value;
        _hasDirectDebitFeature = hasDirectDebitFeature;

        if (hasDirectDebitFeature is true)
        {
            if (providerId is null || string.IsNullOrEmpty(ddBankCode) || maxWithdrawalAmountPerDay is null || maxMandateValidityDurationPerMonth is null || authenticationType is null)
            {
                throw new RequiredDirectDebitSettingException(Id);
            }
            
            if (DirectDebitSetting is null)
            {
                DirectDebitSetting = BankDirectDebitSetting.Create((long)providerId, ddBankCode, (decimal)maxWithdrawalAmountPerDay,
                    (ValidityDuration)maxMandateValidityDurationPerMonth, (AuthenticationType)authenticationType);
            }
            else
            {
                DirectDebitSetting.Update((long)providerId, ddBankCode, (decimal)maxWithdrawalAmountPerDay,
                    (ValidityDuration)maxMandateValidityDurationPerMonth, (AuthenticationType)authenticationType);
            }
        }
        else
        {
            DirectDebitSetting = null;
        }
    }

    public void SetAsActive()
    {
        if (IsActive == true)
            throw new BankIsActiveException(Id);

        IsActive = true;

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    }

    public void SetAsInactive()
    {
        if (IsActive == false)
            throw new BankIsNotActiveException(Id);

        IsActive = false;

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    }
}
