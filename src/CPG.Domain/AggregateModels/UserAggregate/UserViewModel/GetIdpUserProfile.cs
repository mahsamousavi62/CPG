namespace CPG.Domain.AggregateModels.UserAggregate.UserViewModel;

public class GetIdpUserProfileModel(string idpId, string authority, string clientId, string clientSecret, string scope, string idpGetProfileUrl)
{
    public string IdpId { get; set; } = idpId;
    public string Authority { get; set; } = authority;
    public string ClientId { get; set; } = clientId;
    public string ClientSecret { get; set; } = clientSecret;
    public string Scope { get; set; } = scope;
    public string IdpGetProfileUrl { get; set; } = idpGetProfileUrl;

}
