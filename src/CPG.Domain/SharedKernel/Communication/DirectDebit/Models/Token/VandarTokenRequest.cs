using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;

public class VandarTokenRequest : VandarRequestBase
{
    [JsonProperty("refreshtoken")]
    public string RefreshToken { get; set; }
}
