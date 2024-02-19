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
using CPG.Domain.SharedKernel.ApplicationSettings;
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

namespace CPG.Infrastructure.Providers.NeoBank;

public class NeoBankProvider(IHttpClientFactory factory, IConfiguration configuration, IAuthService authService,
    IHttpContextAccessor httpContextAccessor) : INeoBankService
{
    private readonly IHttpClientFactory factory = factory;
    private readonly IConfiguration configuration = configuration;
    private readonly IAuthService authService = authService;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result<UserDepositBalanceResponse>> GetUserDepositBalance()
    {
        var neobankConfig = configuration.GetSection("Infrastructure:NeoBank").Get<NeoBankConfig>();
        ResultData<UserDepositBalanceResponse> resultData = new();
        var appConfig = authService.GetJwtConfig();

        var accessTokenResult = await ExchangeToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
        return Result<UserDepositBalanceResponse>.Failure(new Error("2201001",  accessTokenResult.Error));
          
        try
        {
            var client = factory.CreateClient("neoBankClient");
            client.SetBearerToken(accessTokenResult.Data.AccessToken);
            var result = await client.GetAsync(neobankConfig.UserDepositBalanceUrl);
            if (!result.IsSuccessStatusCode)
                return Result<UserDepositBalanceResponse>.Failure(new Error("2201001", ReasonPhrases.GetReasonPhrase((int)result.StatusCode)));

            var resultContent = await result.Content.ReadAsStringAsync();
            try
            {
                var response= JsonConvert.DeserializeObject<ResultData<UserDepositBalanceResponse>>(resultContent);
                if (response.OperationResult != Enums.OperationResult.Succeeded)
                    return Result<UserDepositBalanceResponse>.Failure(new Error("2201001", GlobalResource.UserHasnotCharismaCart));
                else
                    return Result<UserDepositBalanceResponse>.SuccessResult(new UserDepositBalanceResponse{Balance = response.Data.Balance});
            }
            catch (Exception)
            {
                return Result<UserDepositBalanceResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }
        }
        catch (Exception)
        {
            return Result<UserDepositBalanceResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
        }
    }
    async Task<ResultData<TokenResponse>> ExchangeToken(JwtConfigViewModel appConfig)

    {
        var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        var client = new HttpClient();

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


