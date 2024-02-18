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
        public string? Url { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
