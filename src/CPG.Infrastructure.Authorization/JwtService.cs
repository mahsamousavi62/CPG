using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CPG.Application.Auth;
using CPG.Domain.SharedKernel.ApplicationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CPG.Infrastructure.Authorization;

public class JwtService(IConfiguration config) : IAuthService
{
    private readonly string _secret = config.GetSection("JwtConfig").GetSection("Secret").Value;
    private readonly string _expDate = config.GetSection("JwtConfig").GetSection("ExpirationInMinutes").Value;

    public string GenerateSecurityToken(long userId, string email, string name)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, userId.ToString()), 
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name)
            }),
            Audience = "localhost",
            Issuer = "localhost",
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public JwtConfigViewModel GetJwtConfig()
    {
        return config.GetSection("JwtConfig").Get<JwtConfigViewModel>();
    }
}
