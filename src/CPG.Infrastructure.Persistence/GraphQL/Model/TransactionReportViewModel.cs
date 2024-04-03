using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;  
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.GraphQL.Model
{
    public class TransactionReportViewModel
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string CompanyPersianName { get; set; }
        public string CompanyEnglishName { get; set; }
        public string CompanyLogo { get; set; }
        public long PaymentRquestId { get; set; }
        public long IPGTransactionId { get; set; }
        public long DirectDebitTransactionId { get; set; }
        public long CharismaCardTransactionId { get; set; }
        public Enums.TransactionType TransactionMethodType { get; set; }
        public string TransactionMethodTypeName { get; set; }
        public string IpgTypeName { get; set; }
        public string ProviderName { get; set; }
        public long DestinationDepositId { get; set; }
        public decimal Amount { get; set; }
        public long ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public DateTime PredictedSettlementDateTime { get; set; }
        public Enums.TransactionStatus Status { get; set; }
        public string PaymentCode { get; set; }
        public string CompanyDepositName { get; set; }
        public string CompanyDepositaccountNumber { get; set; }
        public string CompanyDepositIban { get; set; }
        public string? ReferenceNumber { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime? TransactionCreateDateTime { get; set; }
        public DateTime? TransactionModificationDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
    }
}
