using CPG.Domain.AggregateModels.CompanyAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels
{
    public class CompanyDepositReadModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int BankId { get; set; }
        public string Iban { get; set; }
        public string AccountNumber { get; set; }
        public long CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public BankReadModel Bank { get; set; }
        public CompanyReadModel Company { get; set; }
    }
}
