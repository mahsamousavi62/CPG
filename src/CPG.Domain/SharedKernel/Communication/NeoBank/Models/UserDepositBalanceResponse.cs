using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.NeoBank.Models
{
    public class UserDepositBalanceResponse
    {
        [JsonPropertyName("balance")]
        public decimal? Balance { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string Iban { get; set; }

        public string CardNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string DepositNumber { get; set; }
        public Enums.NeoBankDepositStatus DepositStatus { get; set; }
    }
}
