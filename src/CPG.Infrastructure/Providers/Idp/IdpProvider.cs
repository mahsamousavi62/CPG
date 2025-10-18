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
            var httpContext = _httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = httpContext?.TraceIdentifier,
                LogId = $"{Guid.NewGuid()} - Idp - {disco.Exception?.GetType().Name ?? "DiscoveryError"}",
                RequestId = httpContext?.TraceIdentifier,
                AuditLevel = 3, // Error
                AuditType = Enums.AuditType.Provider,
                ServiceName = "GetIdpToken",
                ProviderName = "Idp",
                RequestUri = appConfig.Authority,
                RequestHeader = null,
                RequestBody = System.Text.Json.JsonSerializer.Serialize(new { Address = appConfig.Authority }),
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = disco.Error,
                ApplicationId = applicationId == 0 ? null : applicationId,
                UserId = UserId == 0 ? null : UserId,
                Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
                CompanyId = companyId == 0 ? null : companyId,
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                Response = disco.Error,
                ErrorCode = disco.Exception?.GetType().Name,
                IsSucceeded = false,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now,
                DurationMs = 0,
                StackTrace = disco.Exception?.StackTrace,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = appConfig.Authority,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.GetIdpToken,
                CreationDate = DateTime.Now,
                CreationUserId = UserId == 0 ? 1 : UserId,
                ErrorType = disco.Error,
                ProviderType = Enums.ProviderTypeInLog.Idp,
                CorrolationId = httpContext?.TraceIdentifier
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

        var httpContext = _httpContextAccessor.HttpContext;
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
        _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

        var callLog = new CallLogModel
        {
            // New fields (25 required fields)
            CorrelationId = httpContext?.TraceIdentifier,
            LogId = $"{Guid.NewGuid()} - Idp - {(tokenResponse is not null ? "TokenSuccess" : "TokenNull")}",
            RequestId = httpContext?.TraceIdentifier,
            AuditLevel = tokenResponse is not null ? 1 : 2, // Info or Warning
            AuditType = Enums.AuditType.Provider,
            ServiceName = "GetIdpToken",
            ProviderName = "Idp",
            RequestUri = disco.TokenEndpoint,
            RequestHeader = null,
            RequestBody = System.Text.Json.JsonSerializer.Serialize(tokenRequest),
            ResponseStatusCode = tokenResponse is not null ? 200 : 500,
            ResponseHeader = null,
            ResponseBody = System.Text.Json.JsonSerializer.Serialize(tokenResponse),
            ApplicationId = applicationId == 0 ? null : applicationId,
            UserId = UserId == 0 ? null : UserId,
            Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
            CompanyId = companyId == 0 ? null : companyId,
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            Response = System.Text.Json.JsonSerializer.Serialize(tokenResponse),
            ErrorCode = null,
            IsSucceeded = tokenResponse is not null,
            StartDateTime = DateTime.Now,
            EndDateTime = DateTime.Now,
            DurationMs = 0,
            StackTrace = null,

            // Original fields (preserved)
            ServiceCallDate = DateTime.Now,
            ServiceCallUrl = disco.TokenEndpoint,
            ServiceCallStatus = tokenResponse is not null ? true : false,
            ServiceType = Enums.ServiceType.GetIdpToken,
            CreationDate = DateTime.Now,
            CreationUserId = UserId == 0 ? 1 : UserId,
            ErrorType = null,
            ProviderType = Enums.ProviderTypeInLog.Idp,
            CorrolationId = httpContext?.TraceIdentifier
        };

        _logService.LogInformation(callLog);

        if (tokenResponse.IsError)
        {
            var errorCallLog = new CallLogModel
            {
                // New fields (25 required fields)
                CorrelationId = httpContext?.TraceIdentifier,
                LogId = $"{Guid.NewGuid()} - Idp - TokenError",
                RequestId = httpContext?.TraceIdentifier,
                AuditLevel = 3, // Error
                AuditType = Enums.AuditType.Provider,
                ServiceName = "GetIdpToken",
                ProviderName = "Idp",
                RequestUri = disco.TokenEndpoint,
                RequestHeader = null,
                RequestBody = System.Text.Json.JsonSerializer.Serialize(tokenRequest),
                ResponseStatusCode = 500,
                ResponseHeader = null,
                ResponseBody = $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}",
                ApplicationId = applicationId == 0 ? null : applicationId,
                UserId = UserId == 0 ? null : UserId,
                Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
                CompanyId = companyId == 0 ? null : companyId,
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                Response = $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}",
                ErrorCode = "TokenError",
                IsSucceeded = false,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now,
                DurationMs = 0,
                StackTrace = null,

                // Original fields (preserved)
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = disco.TokenEndpoint,
                ServiceCallStatus = false,
                ServiceType = Enums.ServiceType.GetIdpToken,
                CreationDate = DateTime.Now,
                CreationUserId = UserId == 0 ? 1 : UserId,
                ErrorType = tokenResponse.Error,
                ProviderType = Enums.ProviderTypeInLog.Idp,
                CorrolationId = httpContext?.TraceIdentifier
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

