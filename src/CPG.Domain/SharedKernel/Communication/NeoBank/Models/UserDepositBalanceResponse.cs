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
        [JsonPropertyName("data")]
        public ResponseData Data { get; set; }

        [JsonPropertyName("message")]
        public object Message { get; set; }

        [JsonPropertyName("action")]
        public object Action { get; set; }

        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }

        [JsonPropertyName("errors")]
        public object Errors { get; set; }
    }

    public class ResponseData
    {
        [JsonPropertyName("balance")]
        public int Balance { get; set; }

        [JsonPropertyName("url")]
        public object Url { get; set; }

        [JsonPropertyName("userName")]
        public object UserName { get; set; }

        [JsonPropertyName("password")]
        public object Password { get; set; }
    }
}
