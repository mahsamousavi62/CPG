using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class Bank : AuditableEntity<int>, IAggregateRoot
{
    internal string _name;
    internal string _logoAddress;
    internal IbanPrefix _ibanPrefix;

    public string Name => _name;
    public string LogoAddress => _logoAddress;
    public IbanPrefix IbanPrefix => _ibanPrefix;

    public Bank()
    {
    }

    public void Update(IbanPrefix ibanPrefix)
    {
        _ibanPrefix = ibanPrefix;
        SetModificationData();
    }

    public void SetModificationData()
    {
        ModificationDate = DateTime.Now;
    }

    public void SetAsActive()
    {
        if (IsActive == true)
            throw new BankIsActiveException(Id);

        IsActive = true;
        SetModificationData();

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    }

    public void SetAsInactive()
    {
        if (IsActive == false)
            throw new BankIsNotActiveException(Id);

        IsActive = false;
        SetModificationData();

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, DateTime.Now));
    }
}
