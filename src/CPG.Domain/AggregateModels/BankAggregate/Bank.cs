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

    public void Update(IbanPrefix ibanPrefix)
    {
        _ibanPrefix = ibanPrefix;
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
