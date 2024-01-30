using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;

public class TokenResponse : VandarResponseBase
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public int ExpiresIn { get; set; }

    public string TrackerId { get; set; }
}
