using System.Text.Json.Serialization;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class ValidateGrantRequestViewModel
{
    public string Request { get; set; }
}

public class VandarValidateGrantViewModel
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("authorization_id")]
    public string AuthorizationId { get; set; }
}