using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.NeoBank.Models
{
    public class ClientDirectDebitRequest
    {
        public string TrackerId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string DestinationDepositNumber { get; set; }
        public string Description { get; set; }
    }
}
