using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Interfaces;

public interface IAuthenticationService
{
    Task<IEnumerable<Claim>> GetCurrentClaims();
    Task<ClaimsPrincipal> GetCurrentClaimsPrincipal();
    Task<Guid> GetCurrentSubject(string issuer = null);
    Task<T> GetDataFromClaim<T>(string propertyName, string issuer = null);
    Task<Guid> GetGuidFromClaim(string propertyName, string issuer = null);
    Task<string> GetClientId(string issuer = null);
}