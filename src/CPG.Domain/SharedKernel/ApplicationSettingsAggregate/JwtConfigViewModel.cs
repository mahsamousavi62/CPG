namespace CPG.Domain.SharedKernel.ApplicationSettingsAggregate;

public class JwtConfigViewModel
{
    public string Authority { get; set; }

    public string ClientApiKey { get; set; }

    public string ClientApiSecret { get; set; }

    public string ServerApiKey { get; set; }

    public string ServerApiSecret { get; set; }

    public string ServerScope { get; set; }

    public string ClientId { get; set; }

    public int ClockSkew { get; set; }

    public bool ValidateIssuer { get; set; }

    public bool ValidateAudience { get; set; }

    public bool ValidateLifetime { get; set; }

    public string IdpGetProfileUrl { get; set; }

    public string IdpGetUserStatusUrl { get; set; }

    public int ExpireTime { get; set; }
    
    public string NeoBankScope { get; set; }

}
