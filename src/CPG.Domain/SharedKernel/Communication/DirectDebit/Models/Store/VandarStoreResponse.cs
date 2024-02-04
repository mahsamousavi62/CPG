using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class VandarStoreResponse : VandarResponseBase
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("result")]
    public ResultData Result { get; set; }
}

public class AuthorizationData
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}

public class ResultData
{
    [JsonPropertyName("authorization")]
    public AuthorizationData Authorization { get; set; }
}
