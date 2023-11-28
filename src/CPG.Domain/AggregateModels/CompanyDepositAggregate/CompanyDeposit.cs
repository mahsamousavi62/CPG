using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Events;
using CPG.Domain.SeedWork;
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

        public CompanyDeposit(string name, string iban, int bankId, string accountNumber, long companyId)
        {
            _name = name;
            _iban = iban;
            _bankId = bankId;
            _accountNumber = accountNumber;
            _companyId = companyId;
        }

        public static CompanyDeposit Create(string name, string iban, int bankId, string accountNumber, long companyId)
        {
            var companyDeposit = new CompanyDeposit(name, iban, bankId, accountNumber, companyId);

            companyDeposit.AddDomainEvent(new NewCompanyDepositCreatedEvent(companyDeposit.Id, DateTime.UtcNow));

            return companyDeposit;
        }

    }
}
