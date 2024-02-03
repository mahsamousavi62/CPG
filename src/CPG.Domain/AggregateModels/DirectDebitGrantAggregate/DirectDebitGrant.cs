using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate;

public class DirectDebitGrant : AuditableEntity<long>, IAggregateRoot
{
    internal long _userId;
    internal int _bankId;
    internal string _accountNumber;
    internal string _phoneNumber;
    internal IbanPrefix _ibanPrefix;
    internal bool? _hasDirectDebitFeature;

    public long UserId => _userId;
    public int BankId => _bankId;
    public string AccountNumber => _accountNumber;
    public string PhoneNumber => _phoneNumber;
    public IbanPrefix IbanPrefix => _ibanPrefix;
    public bool? HasDirectDebitFeature => _hasDirectDebitFeature;
    public BankDirectDebitSetting DirectDebitSetting { get; set; }
    public List<CompanyDeposit> CompanyDeposits { get; set; }

    public DirectDebitGrant()
    {
    }

    //public void Update(string name, string logoAddress, IbanPrefix ibanPrefix, bool hasDirectDebitFeature, long? providerId, string ddBankCode, decimal? maxWithdrawalAmountPerDay,
    //    ValidityDuration? maxMandateValidityDurationPerMonth, AuthenticationType? authenticationType)
    //{
    //    _ibanPrefix = ibanPrefix;
    //    _name = name;
    //    _logoAddress = logoAddress;
    //    _hasDirectDebitFeature = hasDirectDebitFeature;

    //    if (hasDirectDebitFeature is true)
    //    {
    //        if (providerId is null || string.IsNullOrEmpty(ddBankCode) || maxWithdrawalAmountPerDay is null || maxMandateValidityDurationPerMonth is null || authenticationType is null)
    //        {
    //            throw new RequiredDirectDebitSettingException(Id);
    //        }

    //        if (DirectDebitSetting is null)
    //        {
    //            DirectDebitSetting = BankDirectDebitSetting.Create((long)providerId, ddBankCode, (decimal)maxWithdrawalAmountPerDay,
    //                (ValidityDuration)maxMandateValidityDurationPerMonth, (AuthenticationType)authenticationType);
    //        }
    //        else
    //        {
    //            DirectDebitSetting.Update((long)providerId, ddBankCode, (decimal)maxWithdrawalAmountPerDay,
    //                (ValidityDuration)maxMandateValidityDurationPerMonth, (AuthenticationType)authenticationType);
    //        }
    //    }
    //    else
    //    {
    //        DirectDebitSetting = null;
    //    }
    //}

    //public void SetAsActive()
    //{
    //    if (IsActive == true)
    //        throw new BankIsActiveException(Id);

    //    IsActive = true;

    //    AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    //}

    //public void SetAsInactive()
    //{
    //    if (IsActive == false)
    //        throw new BankIsNotActiveException(Id);

    //    IsActive = false;

    //    AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    //}
}