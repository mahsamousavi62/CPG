using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using Newtonsoft.Json;

namespace CPG.Domain.SharedKernel.Communication.Idp.Models.UserStatus;

public class UserStatusResponse : IHttpResponse
{
    public Result Result { get; set; }
    public short StatusCode { get; set; }
}

public class Result
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("uniqueIdentifier")]
    public string UniqueIdentifier { get; set; }

    [JsonProperty("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonProperty("isSuspended")]
    public bool IsSuspended { get; set; }

    [JsonProperty("loginType")]
    public string LoginType { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
    public short StatusCode { get; set; }
}