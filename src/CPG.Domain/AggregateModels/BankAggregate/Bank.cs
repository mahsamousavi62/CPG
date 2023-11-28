using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class Bank : AuditableEntity<int>, IAggregateRoot
{
    internal string _name;
    internal string _logoAddress;
    internal IbanPrefix _ibanPrefix;

    public string Name => _name;
    public string LogoAddress => _logoAddress;
    public IbanPrefix IbanPrefix => _ibanPrefix;
    public List<CompanyDeposit> CompanyDeposits { get; set; }

    public Bank()
    {
    }

    public void Update(IbanPrefix ibanPrefix, long userId)
    {
        _ibanPrefix = ibanPrefix;
        SetModificationData(userId);
    }

    public void SetModificationData(long userId)
    {
        ModificationDate = DateTime.Now;
        ModificationUserId = userId;            
    }

    public void SetAsActive(long userId)
    {
        if (IsActive == true)
            throw new BankIsActiveException(Id);

        IsActive = true;
        SetModificationData(userId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, userId, DateTime.Now));
    }

    public void SetAsInactive(long userId)
    {
        if (IsActive == false)
            throw new BankIsNotActiveException(Id);

        IsActive = false;
        SetModificationData(userId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, userId, DateTime.Now));
    }
}
