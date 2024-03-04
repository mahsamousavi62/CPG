    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.NeoBank.Models
{
    public class ClientDirectDebitResponse
    {
        public long UserDepositId { get; set; }
        public string? AccountNumber { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime TranactionDate { get; set; }
        public string? TranactionId { get; set; }
        public Enums.NeoBankTransferType? TransferType { get; set; }
        public decimal Amount { get; set; }
        public string? ErrorTitle { get; set; }
        public string? ErrorCode { get; set; }
        public string? Name { get; set; }
        public string? TrackerId { get; set; }
        public Enums.NeoBankTransferStatus? TransferStatus { get; set; }
    }
}
