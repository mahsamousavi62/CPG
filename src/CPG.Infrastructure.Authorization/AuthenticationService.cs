using CPG.Domain.SharedKernel.ApplicationSettings;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using IAuthenticationService = CPG.Domain.SharedKernel.IAuthenticationService;
namespace CPG.Infrastructure.Authorization
{
    public class AuthenticationService : IAuthenticationService
    {
        #region [ Private Field(s) ]
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationSettingsRepository _applicationSettingsRepository;
        #endregion

        #region [ Public Method(s) ]
        public AuthenticationService(IHttpContextAccessor httpContextAccessor,
                                     IApplicationSettingsRepository applicationSettingsRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _applicationSettingsRepository = applicationSettingsRepository;
        }
        public async Task<IEnumerable<Claim>> GetCurrentClaims()
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            return await Task.FromResult(claims);
        }

        public async Task<ClaimsPrincipal> GetCurrentClaimsPrincipal()
        {
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            if (authenticateResult.Succeeded)
                return authenticateResult.Principal;
            return null;
        }

        public async Task<Guid> GetGuidFromClaim(string propertyName, string issuer = null)
        {
            var applicationConfigViewModel = await _applicationSettingsRepository.GetAllApplicationSettings();
            if (string.IsNullOrEmpty(issuer))
                issuer = applicationConfigViewModel.Authority;

            var str = await GetDataFromClaim<string>(propertyName, issuer);
            Guid.TryParse(str, out Guid guid);
            return guid;
        }

        public async Task<T> GetDataFromClaim<T>(string propertyName, string issuer = null)
        {
            Claim claim;
            if (_httpContextAccessor.HttpContext.User != null)
            {
                var t = _httpContextAccessor.HttpContext.User.Claims.ToList();
                var tt = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == propertyName);
                claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == propertyName);
            }
            else
            {
                var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
                if (!authenticateResult.Succeeded)
                    return default(T);

                claim = authenticateResult.Principal.FindFirst(c => c.Type == propertyName);
                  
            }

            if (claim != null)
                return (T)ChangeType(typeof(T), claim.Value);

            return default(T);
        }

        public async Task<Guid> GetCurrentSubject(string issuer = null)
        {
            var applicationConfigViewModel = await _applicationSettingsRepository.GetAllApplicationSettings();
            if (string.IsNullOrEmpty(issuer))
                issuer = applicationConfigViewModel.Authority;

            return await GetDataFromClaim<Guid>(JwtClaimTypes.Subject, issuer);
        }
        #endregion

        #region [ Private Method(s) ]
        public async Task<string> GetClientId(string issuer = null)
        {
            var applicationConfigViewModel = await _applicationSettingsRepository.GetAllApplicationSettings();
            if (string.IsNullOrEmpty(issuer))
                issuer = applicationConfigViewModel.Authority;

            return await GetDataFromClaim<string>("client_id", issuer);
        }

        private object ChangeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }
        #endregion
    }
}
