using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel;
using System.Net;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using CPG.Domain.SharedKernel.Communication.Charispay.Models;
using CPG.Infrastructure.Providers.Charispay;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using Microsoft.Identity.Client;
using CPG.Application.Auth;
using IdentityModel;
using Polly;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using Serilog;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel.Logging;
using Serilog.Context;
using System.Collections.Generic;
using System.Linq;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Infrastructure.Providers.NeoBank;

public class NeoBankProvider(IHttpClientFactory factory, IConfiguration configuration, IAuthService authService,
    IHttpContextAccessor httpContextAccessor, ILogService logService, ICurrentUser currentUser) : INeoBankService
{
    private readonly IHttpClientFactory factory = factory;
    private readonly IConfiguration configuration = configuration;
    private readonly IAuthService authService = authService;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ILogService _logService = logService;
    private readonly ICurrentUser currentUser = currentUser;
    public async Task<Result<ClientDirectDebitResponse>> ClientDirectDebit(ClientDirectDebitRequest model)
    {
        var neobankConfig = configuration.GetSection("Infrastructure:NeoBank").Get<NeoBankConfig>();
        ResultData<ClientDirectDebitResponse> resultData = new();
        var appConfig = authService.GetJwtConfig();

        var accessTokenResult = await ExchangeToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
            return Result<ClientDirectDebitResponse>.Failure(new Error("2201001", accessTokenResult.Error));

        try
        {
            var client = factory.CreateClient("neoBankClient");
            client.SetBearerToken(accessTokenResult.Data.AccessToken);
            string json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(neobankConfig.ClientDirectDebit, content);
            if (!result.IsSuccessStatusCode)
                return Result<ClientDirectDebitResponse>.Failure(new Error("2201001", ReasonPhrases.GetReasonPhrase((int)result.StatusCode)));

            var resultContent = await result.Content.ReadAsStringAsync();

            result.Headers.TryGetValues("x-correlation-id", out IEnumerable<string> res);
            var neoBankCorroletionId = res?.FirstOrDefault();

            var response = JsonConvert.DeserializeObject<ResultData<ClientDirectDebitResponse>>(resultContent);

            var httpContext = httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateSuccess(
                serviceName: Enums.ServiceType.ClientDirectDebit.ToString(),
                providerName: "NeoBank",
                requestUri: neobankConfig.ClientDirectDebit,
                requestBody: System.Text.Json.JsonSerializer.Serialize(model),
                responseBody: resultContent,
                serviceType: Enums.ServiceType.ClientDirectDebit,
                providerType: Enums.ProviderTypeInLog.NeoBank,
                auditType: Enums.AuditType.Provider,
                correlationId: neoBankCorroletionId ?? httpContext?.TraceIdentifier,
                userId: currentUser.UserId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                responseStatusCode: (int)result.StatusCode
            );

            _logService.LogInformation(callLog);


            switch (response.Data.ErrorCode)
            {
                case "01":
                    return Result<ClientDirectDebitResponse>.Failure(new Error("2202002", GlobalResource.DestinationAccountDoesNotBelong));
                case "02":
                    return Result<ClientDirectDebitResponse>.Failure(new Error("2202003", GlobalResource.AccountDoesNotHaveEnoughBalance));
                case "99":
                    return Result<ClientDirectDebitResponse>.Failure(new Error("2202004", GlobalResource.ServiceDisrupted));
                default:
                    return Result<ClientDirectDebitResponse>.SuccessResult(response.Data);
            }
        }
        catch (Exception ex)
        {
            var httpContext = httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: Enums.ServiceType.ClientDirectDebit.ToString(),
                providerName: "NeoBank",
                requestUri: neobankConfig.ClientDirectDebit,
                requestBody: System.Text.Json.JsonSerializer.Serialize(model),
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.ClientDirectDebit,
                providerType: Enums.ProviderTypeInLog.NeoBank,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: currentUser.UserId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogError(callLog);

            return Result<ClientDirectDebitResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
        }
    }

    public async Task<Result<UserDepositBalanceResponse>> GetUserDepositBalance()
    {
        var neobankConfig = configuration.GetSection("Infrastructure:NeoBank").Get<NeoBankConfig>();
        var appConfig = authService.GetJwtConfig();
        var accessTokenResult = await ExchangeToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
            return Result<UserDepositBalanceResponse>.Failure(new Error("2201001", accessTokenResult.Error));

        try
        {
            var client = factory.CreateClient("neoBankClient");
            client.SetBearerToken(accessTokenResult.Data.AccessToken);
            var result = await client.PostAsync(neobankConfig.UserDepositBalanceUrl, null);
            if (!result.IsSuccessStatusCode)
                return Result<UserDepositBalanceResponse>.Failure(new Error("2201001", ReasonPhrases.GetReasonPhrase((int)result.StatusCode)));

            var resultContent = await result.Content.ReadAsStringAsync();

            result.Headers.TryGetValues("x-correlation-id", out IEnumerable<string> res);
            var neoBankCorroletionId = res?.FirstOrDefault();

            try
            {
                var response = JsonConvert.DeserializeObject<ResultData<UserDepositBalanceResponse>>(resultContent);

                var httpContext = httpContextAccessor.HttpContext;
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

                var callLog = CallLogModel.CreateSuccess(
                    serviceName: Enums.ServiceType.GetUserDepositBalance.ToString(),
                    providerName: "NeoBank",
                    requestUri: neobankConfig.UserDepositBalanceUrl,
                    requestBody: "",
                    responseBody: resultContent,
                    serviceType: Enums.ServiceType.GetUserDepositBalance,
                    providerType: Enums.ProviderTypeInLog.NeoBank,
                    auditType: Enums.AuditType.Provider,
                    correlationId: neoBankCorroletionId ?? httpContext?.TraceIdentifier,
                    userId: currentUser.UserId,
                    applicationId: applicationId,
                    companyId: companyId,
                    ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                    userAgent: httpContext?.Request.Headers["User-Agent"].ToString(),
                    responseStatusCode: (int)result.StatusCode
                );

                _logService.LogInformation(callLog);

                if (response.OperationResult == Enums.OperationResult.Succeeded)
                {
                    return Result<UserDepositBalanceResponse>.SuccessResult(new UserDepositBalanceResponse
                    {
                        Balance = response.Data.Balance,
                        CardNumber = response.Data.CardNumber,
                        CustomerFirstName = response.Data.CustomerFirstName,
                        CustomerLastName = response.Data.CustomerLastName,
                        DepositNumber = response.Data.DepositNumber,
                        DepositStatus = response.Data.DepositStatus,
                        ExpirationDate = response.Data.ExpirationDate,
                        Iban = response.Data.Iban,
                    });
                }
                else
                {
                    var httpContextError = httpContextAccessor.HttpContext;
                    _ = long.TryParse(httpContextError?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long appId);
                    _ = long.TryParse(httpContextError?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long compId);

                    var errorCallLog = CallLogModel.CreateError(
                        serviceName: Enums.ServiceType.GetUserDepositBalance.ToString(),
                        providerName: "NeoBank",
                        requestUri: neobankConfig.UserDepositBalanceUrl,
                        requestBody: "",
                        responseBody: response.Error,
                        exception: null,
                        serviceType: Enums.ServiceType.GetUserDepositBalance,
                        providerType: Enums.ProviderTypeInLog.NeoBank,
                        auditType: Enums.AuditType.Provider,
                        correlationId: httpContextError?.TraceIdentifier,
                        userId: currentUser.UserId,
                        applicationId: appId,
                        companyId: compId,
                        ip: httpContextError?.Connection.RemoteIpAddress?.ToString(),
                        userAgent: httpContextError?.Request.Headers["User-Agent"].ToString()
                    );
                    _logService.LogError(errorCallLog);
                    return Result<UserDepositBalanceResponse>.Failure(new Error("2201000", response.Error));
                }
            }
            catch (Exception ex)
            {
                var httpContext = httpContextAccessor.HttpContext;
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
                _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

                var callLog = CallLogModel.CreateError(
                    serviceName: Enums.ServiceType.GetUserDepositBalance.ToString(),
                    providerName: "NeoBank",
                    requestUri: neobankConfig.UserDepositBalanceUrl,
                    requestBody: "",
                    responseBody: ex.Message,
                    exception: ex,
                    serviceType: Enums.ServiceType.GetUserDepositBalance,
                    providerType: Enums.ProviderTypeInLog.NeoBank,
                    auditType: Enums.AuditType.Provider,
                    correlationId: httpContext?.TraceIdentifier,
                    userId: currentUser.UserId,
                    applicationId: applicationId,
                    companyId: companyId,
                    ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                    userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
                );
                _logService.LogError(callLog);
                return Result<UserDepositBalanceResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }
        }
        catch (Exception ex)
        {
            var httpContext = httpContextAccessor.HttpContext;
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);
            _ = long.TryParse(httpContext?.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out long companyId);

            var callLog = CallLogModel.CreateError(
                serviceName: Enums.ServiceType.GetUserDepositBalance.ToString(),
                providerName: "NeoBank",
                requestUri: neobankConfig.UserDepositBalanceUrl,
                requestBody: "",
                responseBody: ex.Message,
                exception: ex,
                serviceType: Enums.ServiceType.GetUserDepositBalance,
                providerType: Enums.ProviderTypeInLog.NeoBank,
                auditType: Enums.AuditType.Provider,
                correlationId: httpContext?.TraceIdentifier,
                userId: currentUser.UserId,
                applicationId: applicationId,
                companyId: companyId,
                ip: httpContext?.Connection.RemoteIpAddress?.ToString(),
                userAgent: httpContext?.Request.Headers["User-Agent"].ToString()
            );
            _logService.LogError(callLog);
            return Result<UserDepositBalanceResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
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
            Scope = appConfig.NeoBankScope,
            SubjectToken = token,
            SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,

            Parameters =
        {
            { "exchange_style", "impersonation" }
        }
        };
        var response = await client.RequestTokenExchangeTokenAsync(request);

        if (response.IsError)
            throw new Exception(response.Error);

        return new ResultData<TokenResponse> { Data = response, OperationResult = Enums.OperationResult.Succeeded };
    }

}


