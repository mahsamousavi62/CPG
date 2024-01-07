using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels
{
    public class PaymentRequestReadModel
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long ApplicationId { get; set; }
        public string DestinationDepositIban { get; set; }
        public string NationalCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string CallBackUrl { get; set; }
        public string PaymentCode { get; set; }
        public string TrackerId { get; set; }
        public Enums.PaymentStatus Status { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? VerificationDateTime { get; set; }
        public DateTime UrlExpirationDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public ApplicationReadModel Application { get; set; }
        public CompanyReadModel Company { get; set; }
        public TransactionReadModel Transaction { get; set; }
    }
}
