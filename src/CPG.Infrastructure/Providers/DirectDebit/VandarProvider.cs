using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using CPG.Domain.SharedKernel.Helper;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.DirectDebit;

internal class VandarProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository, IRedisCacheService cacheService) : IDirectDebitProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IRedisCacheService _cacheService = cacheService;
    private readonly IHttpProvider _httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private string refreshToken;
    public const string tokenCacheKey = "vandar_token_key";

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            refreshToken = jsonObjectProviderData["Refresh_Token"] is not null ? jsonObjectProviderData["Refresh_Token"] : throw new Exception("Invalid Refresh_Token");
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request)
    {
        var data = _cacheService.GetData<VandarTokenResponse>(tokenCacheKey);
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        if (data is null)
        {
            GetDataFromJsonProvider(request.ProviderData);
            data = await _httpProvider.PostAsync<TokenRequest, VandarTokenResponse,
                                                        VandarResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                        {
                                                            BaseAddress = "https://api.vandar.io/",
                                                            Uri = "v3/refreshtoken",
                                                            Body = new VandarTokenRequest
                                                            {
                                                                RefreshToken = refreshToken,
                                                            },
                                                            Provider = Enums.ProviderType.Sep,
                                                            Service = Enums.ServiceType.SepToken,
                                                        }, request, PaymentTokenErrorHandler);
            _cacheService.SetData(tokenCacheKey, data);
        }
        return new TokenResponse
        {
            AccessToken = data.AccessToken,
            RefreshToken = data.RefreshToken,
            ExpiresIn = data.ExpiresIn,
            TrackerId = trackerId,
        };
    }

    private async Task<TResponse> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
        where TResponse : VandarTokenResponse
        where TError : VandarResponseBase
        where TBaseRequest : TokenRequest
    {
        return response.StatusCode switch
        {
            401 => await GetToken(),
            422 => throw new Exception(GlobalResource.EmptyRefreshToken),
            _ => await Retry()
        };

        async Task<TResponse> GetToken()
        {
            _cacheService.SetData<VandarTokenResponse?>(tokenCacheKey, null);
            return await GetTokenAsync(baseRequest) as TResponse;
        }

        async Task<TResponse> Retry()
        {
            if (tokenFailCounter < serviceCallMaxTryCounter)
            {
                tokenFailCounter++;
                return await GetTokenAsync(baseRequest) as TResponse;
            }
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error)) as TResponse;
        }
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(VandarResponseBase error)
        where TResponse : ResponseBase
        where TError : VandarResponseBase
        where TBaseRequest : class
    {
        if (error is null)
        {
            throw new Exception(GlobalResource.ProviderUnexpectedError);
        }

        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }
}
