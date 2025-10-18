using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel;
using System;
using CPG.Domain.SharedKernel.Communication.Idp;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using CPG.Application.Auth;
using CPG.Domain.SharedKernel.Communication;
using static CPG.Domain.SharedKernel.Enums;
using System.Collections.Generic;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.AspNetCore.Http;
using System.Linq;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserStatus;

namespace CPG.Infrastructure.Providers.Idp;
public class IdpProvider(
    IAuthService authService,
    IHttpProvider httpProvider,
    ILogService logService,
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor) : IIdpProvider
{
    private readonly ILogService _logService = logService;
    private readonly IAuthService _authService = authService;
    private readonly IHttpProvider _httpProvider = httpProvider;
    public readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<ResultData<UserProfileResponse>> GetUserProfile(string idpId)
    {
        try
        {
            var appConfig = _authService.GetJwtConfig();

            var accessTokenResult = await GetClientCredentialsToken(appConfig);
            if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
                return new ResultData<UserProfileResponse> { Error = accessTokenResult.Error, OperationResult = Enums.OperationResult.Failed };

            List<(string Key, string Value)> list = [("Authorization", string.Concat("BEARER ", accessTokenResult.Data))];
            IdpProfileRequest request = new() { IdpId = idpId };
            var result = await _httpProvider.GetAsync<IdpProfileRequest, UserProfileResponse, IdpProfileRequest>
            (new HttpProviderRequest<IdpProfileRequest, IdpProfileRequest>
            {
                BaseAddress = appConfig.Authority,
                Request = request,
                Body = request,
                Uri = $"{appConfig!.IdpGetProfileUrl}{idpId}",
                ProviderTypeInLog = Enums.ProviderTypeInLog.Idp,
                HeaderParameters = list,
                Service = Enums.ServiceType.GetIdpProfile
            });

            return result is null
                ? new ResultData<UserProfileResponse>
                {
                    OperationResult = OperationResult.NotFound,
                }
                : new ResultData<UserProfileResponse>
                {
                    Data = result,
                    OperationResult = OperationResult.Succeeded,
                };
        }
        catch (Exception ex)
        {

            return new ResultData<UserProfileResponse>
            {
                OperationResult = OperationResult.Failed,
                Error = ex.Message
            };
        }
    }

    private async Task<ResultData<string>> GetClientCredentialsToken(JwtConfigViewModel appConfig)
    {
        var httpClient = _httpClientFactory.CreateClient("idpClient");

        _ = long.TryParse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out long UserId);

        var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = appConfig.Authority,
            Policy = { RequireHttps = false } // Todo: RequireHttps = true
        });
        if (disco.IsError)
        {
            var callLog = new CallLogModel
            {
                RequestBody = System.Text.Json.JsonSerializer.Serialize(new { Address = appConfig.Authority }),
                ResponseBody = disco.Error,
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = appConfig.Authority,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.GetIdpToken,
                CreationDate = DateTime.Now,
                CreationUserId = UserId == 0 ? 1 : UserId,
                ErrorCode = disco.Exception?.GetType().Name,
                ErrorType = disco.Error,
                ProviderType = Enums.ProviderTypeInLog.Idp,
                AuditType = Enums.AuditType.Provider
            };
            _logService.LogError(callLog);
            return new ResultData<string> { Error = disco.Error, OperationResult = Enums.OperationResult.Failed };
        }

        var tokenRequest = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = appConfig.ServerApiKey,
            ClientSecret = appConfig.ServerApiSecret,
            Scope = appConfig.ServerScope,
        };
        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

        var callLog = new CallLogModel
        {
            RequestBody = System.Text.Json.JsonSerializer.Serialize(tokenRequest),
            ResponseBody = System.Text.Json.JsonSerializer.Serialize(tokenResponse),
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = disco.TokenEndpoint,
            ServiceCallStatus = tokenResponse is not null ? true : false,
            ServiceType = Enums.ServiceType.GetIdpToken,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ProviderType = Enums.ProviderTypeInLog.Idp,
            AuditType = Enums.AuditType.Provider
        };

        _logService.LogInformation(callLog);

        if (tokenResponse.IsError)
        {
            var errorCallLog = new CallLogModel
            {
                RequestBody = System.Text.Json.JsonSerializer.Serialize(tokenRequest),
                ResponseBody = $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}",
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = disco.TokenEndpoint,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.GetIdpToken,
                CreationDate = DateTime.Now,
                CreationUserId = UserId == 0 ? 1 : UserId,
                ErrorCode = "TokenError",
                ErrorType = tokenResponse.Error,
                ProviderType = Enums.ProviderTypeInLog.Idp,
                AuditType = Enums.AuditType.Provider
            };
            _logService.LogError(errorCallLog);
            return new ResultData<string>
            {
                Error = $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}",
                OperationResult = Enums.OperationResult.Failed
            };
        }
        return new ResultData<string>
        {
            Data = tokenResponse.AccessToken,
            OperationResult = Enums.OperationResult.Succeeded
        };
    }

    public async Task<ResultData<UserStatusResponse>> GetUserStatus(string idpId)
    {
        try
        {
            var appConfig = _authService.GetJwtConfig();

            var accessTokenResult = await GetClientCredentialsToken(appConfig);
            if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
                return new ResultData<UserStatusResponse> { Error = accessTokenResult.Error, OperationResult = Enums.OperationResult.Failed };

            List<(string Key, string Value)> list = [("Authorization", string.Concat("BEARER ", accessTokenResult.Data))];
            IdpProfileRequest request = new() { IdpId = idpId };
            var result = await _httpProvider.GetAsync<IdpProfileRequest, UserStatusResponse, IdpProfileRequest>
             (new HttpProviderRequest<IdpProfileRequest, IdpProfileRequest>
             {
                 BaseAddress = appConfig.Authority,
                 Request = request,
                 Body = request,
                 Uri = $"{appConfig!.IdpGetUserStatusUrl}{idpId}?idType=UserId",
                 ProviderTypeInLog = Enums.ProviderTypeInLog.Idp,
                 HeaderParameters = list,
                 Service = Enums.ServiceType.GetIdpUserStatus
             });

            return result is null
                ? new ResultData<UserStatusResponse>
                {
                    OperationResult = OperationResult.NotFound,
                }
                : new ResultData<UserStatusResponse>
                {
                    Data = result,
                    OperationResult = OperationResult.Succeeded,
                };
        }
        catch (Exception ex)
        {
            return new ResultData<UserStatusResponse>
            {
                OperationResult = OperationResult.Failed,
                Error = ex.Message
            };
        }
    }

}

