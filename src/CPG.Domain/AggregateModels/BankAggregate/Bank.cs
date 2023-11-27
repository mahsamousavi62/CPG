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

    public void Update(IbanPrefix ibanPrefix, long cpgUserId)
    {
        _ibanPrefix = ibanPrefix;
        SetModificationData(cpgUserId);
    }

    public void SetModificationData(long UserId)
    {
        ModificationDate = DateTime.Now;
        ModificationUserId = UserId;            
    }

    public void SetAsActive(long UserId)
    {
        if (IsActive == true)
            throw new BankIsActiveException(Id);

        IsActive = true;
        SetModificationData(cpgUserId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, cpgUserId, DateTime.Now));
    }

    public void SetAsInactive(long UserId)
    {
        if (IsActive == false)
            throw new BankIsNotActiveException(Id);

        IsActive = false;
        SetModificationData(cpgUserId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, IsActive, cpgUserId, DateTime.Now));
    }
}
