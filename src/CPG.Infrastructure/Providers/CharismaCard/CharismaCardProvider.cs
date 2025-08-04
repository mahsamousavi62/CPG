using CPG.Application.Auth;
using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.CharismaCard;

public class CharismaCardProvider(
       IHttpClientFactory factory, IConfiguration configuration, IAuthService authService,
    IHttpContextAccessor httpContextAccessor, ILogger<CharismaCardProvider> logger, ICurrentUser currentUser) : ICharismaCardService
{

    private readonly IHttpClientFactory factory = factory;
    private readonly IConfiguration configuration = configuration;
    private readonly IAuthService authService = authService;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ILogger<CharismaCardProvider> logger = logger;
    private readonly ICurrentUser currentUser = currentUser;

    public async Task<Result<CharismaCardUserDepositBalanceResponse>> GetUserDepositBalance(string nationalCode)
    {
        var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();


        var appConfig = authService.GetJwtConfig();

        var accessTokenResult = await ExchangeToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
            return Result<CharismaCardUserDepositBalanceResponse>.Failure(new Error("2201001", accessTokenResult.Error));
        try
        {
            var client = factory.CreateClient("charismaCardClient");

            string query = $"?NationalCode={nationalCode}";

            client.SetBearerToken(accessTokenResult.Data.AccessToken);

            var response = await client.GetAsync(charismaCardConfig.GetUserDepositBalanceUrl + query);

            var responseContent = await response.Content.ReadAsStringAsync();


            var callLog = new CallLogModel
            {

                ResponseBody = responseContent,
                ServiceCallDate = DateTime.Now,
                ServiceCallUrl = charismaCardConfig.GetUserDepositBalanceUrl + query,
                ServiceCallStatus = response.StatusCode == System.Net.HttpStatusCode.OK,
                ServiceType = Enums.ServiceType.GetUserDepositBalance,
                CreationDate = DateTime.Now,
                CreationUserId = currentUser.UserId,
                ProviderType = Enums.ProviderTypeInLog.CharismaCard,
                AuditType = Enums.AuditType.Provider
            };

            using (LogContext.PushProperty("CallLog", callLog, true))
            {
                logger.LogInformation("[CallLog] {@CallLog}", callLog);
            }

            if (response.IsSuccessStatusCode)
            {
                JsonSerializerOptions options = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<CharismaCardUserDepositBalanceResponse>(responseContent, options);

                logger.LogInformation("CharismaCard GetUserDepositBalance successful for user {UserId}. Response: {Response}",
                    currentUser.UserId, responseContent);

                return Result<CharismaCardUserDepositBalanceResponse>.SuccessResult(result);
            }
            else
            {
                logger.LogError("CharismaCard GetUserDepositBalance failed with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);

                return Result<CharismaCardUserDepositBalanceResponse>.Failure(new Error("2451001", GlobalResource.UnexpectedError));
            }
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Exception occurred while calling CharismaCard GetUserDepositBalance");
            return Result<CharismaCardUserDepositBalanceResponse>.Failure(new Error("2451000", GlobalResource.UnexpectedError));
        }
    }


    private async Task<ResultData<TokenResponse>> ExchangeToken(JwtConfigViewModel appConfig)

    {
        var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        var client = factory.CreateClient();
        var httpClient = factory.CreateClient("idpClient");
        var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = appConfig.Authority,
            Policy = { RequireHttps = false }
        });
        if (disco.IsError)
            return new ResultData<TokenResponse> { Error = disco.Error, OperationResult = Enums.OperationResult.Failed };

        var request = new TokenExchangeTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = appConfig.ServerApiKey,
            ClientSecret = appConfig.ServerApiSecret,
            Scope = appConfig.CharismaCardScope,
            SubjectToken = token,
            SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,

            Parameters =
        {
            { "exchange_style", "impersonation" }
        }
        };
        var response = await client.RequestTokenExchangeTokenAsync(request);

        if (response.IsError)
            throw new Exception(response.Raw);

        return new ResultData<TokenResponse> { Data = response, OperationResult = Enums.OperationResult.Succeeded };
    }

    public async Task<Result<DirectDebitResponse>> DirectDebitRequest(DirectDebitRequest request)
    {
        var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();
        var appConfig = authService.GetJwtConfig();

        var accessTokenResult = await ExchangeToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
            return Result<DirectDebitResponse>.Failure(new Error("2201001", accessTokenResult.Error));

        try
        {
            var client = factory.CreateClient("charismaCardClient");

            // Use the hardcoded token for now (same as in GetUserDepositBalance)
            client.SetBearerToken("eyJhbGciOiJSUzI1NiIsImtpZCI6IjYzNzQxNUUwNzgzOTA3NEE5MDU2QjE4QUYxRTdFQ0MzIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2F1dGgtc3RhZ2UuY2hhcmlzbWEuZGlnaXRhbCIsIm5iZiI6MTc1NDI4NTMwMCwiaWF0IjoxNzU0Mjg1MzAwLCJleHAiOjE3NTQyODcxMDAsImF1ZCI6WyJwYXlfX2RhcnlhZnR5YXJfYXBpIiwiaHR0cHM6Ly9hdXRoLXN0YWdlLmNoYXJpc21hLmRpZ2l0YWwvcmVzb3VyY2VzIl0sInNjb3BlIjpbInBheV9fZ2F0ZXdheV9leHRlcm5hbCJdLCJjbGllbnRfaWQiOiJwYXlfX2RhcnlhZnR5YXJfYXBpX2NsaWVudCIsInN1YiI6IjI3In0.NQcXVb2BeJDF-7kFtH1EMVnaboL5Csdn-AJWkIDtwK906dH0iK3ZOO-5x9YH1SYCGzeQIxX35UEqX5vU3ePz-MHHdnznr8VllP81ZQXEzLOjVh59Qv6Zdkz7A9RRk3-JEJuLIeE1k09951SjlcJlSo7M9c3HQXWygyRWAMhXwoXqraO6aaF5k1DdnomEaQJ6tqIgi8OjZRqrUvHVINrSxxyaAjARY-fGEwqDLjc3tkLBIsKpvueXSwRVPkb_UyNf33OycwZG_N7hxyg0jpWwlDN-HupG3prRgd9s2cItdadJ26m4aTFXLp_yRQL5oL8CCrI1hQhXRtFBJrBgXzLOTQ");

            // Serialize the request body
            var jsonRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(charismaCardConfig.DirectDebitRequestUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<DirectDebitResponse>(responseContent, options);

                logger.LogInformation("CharismaCard DirectDebitRequest successful for user {UserId}. Response: {Response}",
                    currentUser.UserId, responseContent);

                return Result<DirectDebitResponse>.SuccessResult(result);
            }
            else
            {
                logger.LogError("CharismaCard DirectDebitRequest failed with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);

                return Result<DirectDebitResponse>.Failure(new Error("2201002", "DirectDebit request failed"));
            }
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Exception occurred while calling CharismaCard DirectDebitRequest");
            return Result<DirectDebitResponse>.Failure(new Error("2201002", "DirectDebit request failed"));
        }
    }
}