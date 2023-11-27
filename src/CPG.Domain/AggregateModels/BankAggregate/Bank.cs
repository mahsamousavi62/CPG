using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class Bank : AuditableEntity<int>, IAggregateRoot
{
    internal BankInformation _bankInformation { get; set; }
    internal BankStatus _status;

    public BankInformation BankInformation => _bankInformation;
    public BankStatus Status => _status;

    public Bank()
    {
    }

    private Bank(BankInformation bankInformation) : this()
    {
        _bankInformation = bankInformation;
        _status = bankInformation.Status;
    }

    public static Bank Create(string name, string swiftCode, BankStatus status, byte[] logo,
                              int providerId, string providerData, decimal directDebitAmountLimit,
                              decimal directDebitDailyTransactionLimit, long UserId)
    {
        var bankInformation = new BankInformation(name, swiftCode, status, logo, providerId,
                                                  providerData, directDebitAmountLimit,
                                                  directDebitDailyTransactionLimit);
        var bank = new Bank(bankInformation);
        bank.CreationDate = DateTime.Now;
        bank.CreationUserId = UserId;

        return bank;
    }

    public static void Update(int id, string name, string swiftCode, BankStatus status, byte[] logo,
                              int providerId, string providerData, decimal directDebitAmountLimit,
                              decimal directDebitDailyTransactionLimit, long UserId)
    {
        
    }

    public void Delete(int id, long UserId)
    {
     //   this.IsDeleted = true;

        SetModificationData(UserId);

        AddDomainEvent(new DeleteBankEvent(Id, UserId, DateTime.Now));
    }

    public void SetModificationData(long UserId)
    {
        ModificationDate = DateTime.Now;
        ModificationUserId = UserId;            
    }

    public void SetAsActive(long UserId)
    {
        if (Status == BankStatus.Active)
            throw new BankIsActiveException(Id);

        _status = BankStatus.Active;
        SetModificationData(UserId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Active, UserId, DateTime.Now));
    }

    public void SetAsInactive(long UserId)
    {
        if (Status == BankStatus.Inactive)
            throw new BankIsNotActiveException(Id);

        _status = BankStatus.Inactive;
        SetModificationData(UserId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Inactive, UserId, DateTime.Now));
    }

    public void SetAsSuspended(long UserId)
    {
        if (Status == BankStatus.Suspended)
            throw new BankIsSuspendedException(Id);

        _status = BankStatus.Suspended;
        SetModificationData(UserId);

        AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Suspended, UserId, DateTime.Now));
    }
}
