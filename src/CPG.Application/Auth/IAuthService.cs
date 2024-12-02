using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;

namespace CPG.Application.Auth;

public interface IAuthService
{
    string GenerateSecurityToken(long userId, string email, string name);

    JwtConfigViewModel GetJwtConfig();
}
