using Newtonsoft.Json;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class ValidateGrantRequestViewModel
{
    public string Request { get; set; }
}

public class VandarValidateGrantViewModel
{
    [JsonProperty("token")]
    public string Token { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("authorization_id")]
    public string AuthorizationId { get; set; }
}