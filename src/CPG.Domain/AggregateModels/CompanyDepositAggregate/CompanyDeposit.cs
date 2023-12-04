using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Events;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CompanyDeposits
{
    public class CompanyDeposit : AuditableEntity<long>, IAggregateRoot
    {
        public CompanyDeposit()
        {
            
        }
        private string _name;
        private string _iban;
        private int _bankId;
        private string _accountNumber;
        private long _companyId;
        public string Name => _name;
        public int BankId => _bankId;
        public string Iban => _iban;
        public string AccountNumber => _accountNumber;
        public long CompanyId => _companyId;

        public Company Company { get; set; }
        public Bank Bank { get; set; }

        public CompanyDeposit(PersianName name, Iban iban, int bankId, string accountNumber, long companyId)
        {
            Guard.Against.NullOrEmpty(accountNumber);

            _name = name.Value;
            _iban = iban.Value;
            _bankId = bankId;
            _accountNumber = accountNumber;
            _companyId = companyId;
            IsActive = true;
        }

        public static CompanyDeposit Create(PersianName name, Iban iban, int bankId, string accountNumber, long companyId)
        {
            var companyDeposit = new CompanyDeposit(name, iban, bankId, accountNumber, companyId);

            companyDeposit.AddDomainEvent(new NewCompanyDepositCreatedEvent(companyDeposit.Id, DateTime.UtcNow));

            return companyDeposit;
        }

    }
}
