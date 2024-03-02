using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels
{
    public class PaymentReceiptTransactionReadModel
    {
        public long Id { get; set; }
        public string SourceIban { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime ReceiptDateTime { get; set; }
        public string Description { get; set; }
        public string ReceiptImage { get; set; }
        public Enums.PaymentReceiptStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
