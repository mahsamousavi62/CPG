
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.NeoBankServices.ViewModels;

public class UserDepositBalanceViewModel
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


