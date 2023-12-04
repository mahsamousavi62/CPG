namespace CPG.Domain.AggregateModels.UserAggregate.UserViewModel
{
    public class GetIdpUserProfileModel
    {
        public GetIdpUserProfileModel(string idpId, string authority, string clientId, string clientSecret, string scope, string idpGetProfileUrl)
        {
            IdpId = idpId;
            Authority = authority;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
            IdpGetProfileUrl = idpGetProfileUrl;
        }

        public string IdpId { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string IdpGetProfileUrl { get; set; }

    }
}
