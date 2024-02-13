using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;

public class VandarVerifyResponse : VandarResponseBase
{
    [JsonProperty("status")]
    public int GrantStatus { get; set; }

    [JsonProperty("message")]
    public string GrantMessage { get; set; }
}
