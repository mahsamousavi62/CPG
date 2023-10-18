using CPG.Domain.AggregateModels.BankAggregate.Events;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate.Events;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate
{
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
                                  decimal directDebitDailyTransactionLimit, long cpgUserId)
        {
            var bankInformation = new BankInformation(name, swiftCode, status, logo, providerId,
                                                      providerData, directDebitAmountLimit,
                                                      directDebitDailyTransactionLimit);
            var bank = new Bank(bankInformation);
            bank.CreationDate = DateTime.Now;
            bank.CreationUserId = cpgUserId;

            return bank;
        }

        public static void Update(int id, string name, string swiftCode, BankStatus status, byte[] logo,
                                  int providerId, string providerData, decimal directDebitAmountLimit,
                                  decimal directDebitDailyTransactionLimit, long cpgUserId)
        {
            
        }

        public void Delete(int id, long cpgUserId)
        {
            this.IsDeleted = true;

            SetModificationData(cpgUserId);

            AddDomainEvent(new DeleteBankEvent(Id, cpgUserId, DateTime.Now));
        }

        public void SetModificationData(long cpgUserId)
        {
            ModificationDate = DateTime.Now;
            ModificationUserId = cpgUserId;            
        }

        public void SetAsActive(long cpgUserId)
        {
            if (Status == BankStatus.Active)
                throw new BankIsActiveException(Id);

            _status = BankStatus.Active;
            SetModificationData(cpgUserId);

            AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Active, cpgUserId, DateTime.Now));
        }

        public void SetAsInactive(long cpgUserId)
        {
            if (Status == BankStatus.Inactive)
                throw new BankIsNotActiveException(Id);

            _status = BankStatus.Inactive;
            SetModificationData(cpgUserId);

            AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Inactive, cpgUserId, DateTime.Now));
        }

        public void SetAsSuspended(long cpgUserId)
        {
            if (Status == BankStatus.Suspended)
                throw new BankIsSuspendedException(Id);

            _status = BankStatus.Suspended;
            SetModificationData(cpgUserId);

            AddDomainEvent(new ChangeBankStatusEvent(Id, BankStatus.Suspended, cpgUserId, DateTime.Now));
        }
    }
}
