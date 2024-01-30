using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;

public class VandarTokenRequest : VandarRequestBase
{
    [JsonPropertyName("refreshtoken")]
    public string RefreshToken { get; set; }
}
