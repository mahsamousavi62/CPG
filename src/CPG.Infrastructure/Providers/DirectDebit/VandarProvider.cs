using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Helper;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.DirectDebit;

internal class VandarProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository,
    IRedisCacheService cacheService) : IDirectDebitProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IRedisCacheService _cacheService = cacheService;
    private readonly IHttpProvider _httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte storeFailCounter = 0;
    private byte showFailCounter = 0;
    private byte getUserGrantsFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private string refreshToken;
    private string businessData;
    public const string tokenCacheKey = "vandar_token_key";

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            refreshToken = jsonObjectProviderData["Refresh_Token"] is not null ? jsonObjectProviderData["Refresh_Token"] : throw new Exception("Invalid Refresh_Token");
            businessData = jsonObjectProviderData["Business_ID"] is not null ? jsonObjectProviderData["Business_ID"] : throw new Exception("Invalid Business_ID");
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request)
    {
        var data = _cacheService.GetData<VandarTokenResponse>(tokenCacheKey);
        if (data is null)
        {
            GetDataFromJsonProvider(request.ProviderData);
            data = await _httpProvider.PostAsync<TokenRequest, VandarTokenResponse, VandarResponseBase, dynamic>(new HttpProviderRequest<dynamic>
            {
                BaseAddress = "https://api.vandar.io/",
                Uri = "v3/refreshtoken",
                Body = new VandarTokenRequest
                {
                    RefreshToken = refreshToken,
                },
                Provider = Enums.ProviderType.Vandar,
                Service = Enums.ServiceType.VandarToken,
            }, request, PaymentTokenErrorHandler, (string stringResponse) =>
            {
                if (stringResponse.Contains("Unauthorized"))
                {
                    stringResponse = "{\"status\": 101, \"error\" :\"Unauthorized\"}";
                }

                return System.Text.Json.JsonSerializer.Deserialize<VandarTokenResponse>(stringResponse);
            });
            _cacheService.SetData(tokenCacheKey, data);
        }
        return new TokenResponse
        {
            AccessToken = data.AccessToken,
            RefreshToken = data.RefreshToken,
            ExpiresIn = data.ExpiresIn,
        };
    }

    public async Task<StoreResponse> StoreAsync(StoreRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = await GetHeaders(request.AccessToken);
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        var applicationSettings = await _applicationSettingRepositoy.GetAllApplicationSettings();

        var callbackUrl = CreateCallbackUrl(applicationSettings.Direct_Debit_Grant_Result_URL, trackerId);
        var data = await _httpProvider.PostAsync<StoreRequest, VandarStoreResponse, VandarResponseBase, dynamic>
            (new HttpProviderRequest<dynamic>
            {
                BaseAddress = "https://api.vandar.io/",
                Uri = $"v3/business/{businessData}/subscription/authorization/store",
                HeaderParameters = headers,
                Body = new VandarStoreRequest
                {
                    BankCode = request.BankCode,
                    CallbackUrl = callbackUrl,
                    Count = request.Count,
                    Limit = request.Limit,
                    Mobile = request.MobileNumber,
                    Name = request.FullName,
                    NationalCode = request.NationalCode,
                    ExpirationDate = request.ExpirationDate.ToString("yyyy-MM-dd")
                },
                Provider = Enums.ProviderType.Vandar,
                Service = Enums.ServiceType.VandarStore,
            }, request, StoreErrorHandler);

        return new StoreResponse
        {
            Status = data.Status,
            Message = data.Message,
            Token = data.Result.Authorization.Token,
            TrackerId = trackerId,
        };
    }

    public async Task<ShowResponse> ShowAsync(ShowRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = await GetHeaders(request.AccessToken);

        var data = await _httpProvider.GetAsync<ShowRequest, VandarShowResponse, VandarResponseBase, dynamic>
            (new HttpProviderRequest<dynamic>
            {
                BaseAddress = "https://api.vandar.io/",
                Uri = $"v3/business/{businessData}/subscription/authorization/{request.AuthorizationId}",
                HeaderParameters = headers,
                Provider = Enums.ProviderType.Vandar,
                Service = Enums.ServiceType.VandarShow,
            }, request, ShowErrorHandler);

        return new ShowResponse
        {
            GrantStatus = data.GrantStatus,
            GrantMessage = data.GrantMessage,
            Status = data.Status,
            StatusCode = data.StatusCode,
            GrantData = new GrantData
            {
                Id = data.Result?.Authorizations?.Id,
                AccountNumber = data.Result?.Authorizations?.PayerAccount?.AccountNumber,
                Status = data.Result?.Authorizations?.Status,
            }
        };
    }

    public async Task<UserGrantsResponse> GetUserGrants(UserGrantsRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = await GetHeaders(request.AccessToken);

        var data = await _httpProvider.PostAsync<UserGrantsRequest, VandarShowMobileResponse, VandarResponseBase, dynamic>
            (new HttpProviderRequest<dynamic>
            {
                BaseAddress = "https://api.vandar.io/",
                Uri = $"v3/business/{businessData}/subscription/authorization?{request.MobileNumber}",
                HeaderParameters = headers,
                Provider = Enums.ProviderType.Vandar,
                Service = Enums.ServiceType.VandarShow,
            }, request, GetUserGrantsErrorHandler);

        return new UserGrantsResponse
        {
            GrantMessage = data.GrantMessage,
            Status = data.Status,
            StatusCode = data.StatusCode,
            Grants = data.Data.Select(t => new GrantData
            {
                Id = t.Id,
                AccountNumber = t.PayerAccount?.AccountNumber,
                Status = t.Status,
            }).ToList(),
        };
    }

    public async Task<VerifyResponse> VerifyAsync(VerifyRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = await GetHeaders(request.AccessToken);

        var data = await _httpProvider.PatchAsync<VerifyRequest, VandarVerifyResponse, VandarResponseBase, dynamic>
            (new HttpProviderRequest<dynamic>
            {
                BaseAddress = "https://api.vandar.io/",
                Uri = $"v3/business/{businessData}/subscription/authorization/{request.AuthorizationId}/verify",
                HeaderParameters = headers,
                Provider = Enums.ProviderType.Vandar,
                Service = Enums.ServiceType.VandarVerify,
            }, request, VerifyErrorHandler);

        return new VerifyResponse
        {
            GrantStatus = data.GrantStatus,
            GrantMessage = data.GrantMessage,
            Status = data.Status,
            StatusCode = data.StatusCode,
        };
    }

    private static string CreateCallbackUrl(string url, string trackerId)
    {
        return $"{url}?track_id={trackerId}";
    }

    private async Task<List<(string Key, string? Value)>> GetHeaders(string accessToken)
    {
        return new List<(string Key, string? Value)> { ("Authorization", string.Concat("Bearer ", accessToken)) };
    }

    private async Task<TResponse> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
        where TResponse : VandarTokenResponse
        where TError : VandarResponseBase
        where TBaseRequest : TokenRequest
    {
        return error.StatusCode switch
        {
            401 => await GetToken(),
            422 => throw new Exception(GlobalResource.EmptyRefreshToken),
            101 => throw new Exception(GlobalResource.InvalidRefreshToken),
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
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
        }
    }

    private async Task<TResponse> StoreErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
            where TResponse : VandarStoreResponse
            where TError : VandarResponseBase
            where TBaseRequest : StoreRequest
    {
        return error.StatusCode switch
        {
            0 => await GetToken(),
            _ => await Retry()
        };

        async Task<TResponse> GetToken()
        {
            _cacheService.SetData<VandarTokenResponse?>(tokenCacheKey, null);
            return await GetTokenAsync(new TokenRequest { ProviderData = baseRequest.ProviderData }) as TResponse;
        }

        async Task<TResponse> Retry()
        {
            if (storeFailCounter < serviceCallMaxTryCounter)
            {
                storeFailCounter++;
                return await StoreAsync(baseRequest) as TResponse;
            }
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
        }
    }

    private async Task<TResponse> ShowErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
            where TResponse : VandarShowResponse
            where TError : VandarResponseBase
            where TBaseRequest : ShowRequest
    {
        return error.StatusCode switch
        {
            0 => await GetToken(),
            _ => await Retry()
        };

        async Task<TResponse> GetToken()
        {
            _cacheService.SetData<VandarShowResponse?>(tokenCacheKey, null);
            return await GetTokenAsync(new TokenRequest { ProviderData = baseRequest.ProviderData }) as TResponse;
        }

        async Task<TResponse> Retry()
        {
            if (showFailCounter < serviceCallMaxTryCounter)
            {
                showFailCounter++;
                return await ShowAsync(baseRequest) as TResponse;
            }
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
        }
    }

    private async Task<TResponse> GetUserGrantsErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
           where TResponse : VandarShowMobileResponse
           where TError : VandarResponseBase
           where TBaseRequest : UserGrantsRequest
    {
        return error.StatusCode switch
        {
            0 => await GetToken(),
            _ => await Retry()
        };

        async Task<TResponse> GetToken()
        {
            _cacheService.SetData<VandarShowResponse?>(tokenCacheKey, null);
            return await GetTokenAsync(new TokenRequest { ProviderData = baseRequest.ProviderData }) as TResponse;
        }

        async Task<TResponse> Retry()
        {
            if (getUserGrantsFailCounter < serviceCallMaxTryCounter)
            {
                getUserGrantsFailCounter++;
                return await GetUserGrants(baseRequest) as TResponse;
            }
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
        }
    }

    private async Task<TResponse> VerifyErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest baseRequest, TResponse response, TError error, short statusCode)
            where TResponse : VandarVerifyResponse
            where TError : VandarResponseBase
            where TBaseRequest : VerifyRequest
    {
        return error.StatusCode switch
        {
            0 => await GetToken(),
            _ => await Retry()
        };

        async Task<TResponse> GetToken()
        {
            _cacheService.SetData<VandarVerifyResponse?>(tokenCacheKey, null);
            return await GetTokenAsync(new TokenRequest { ProviderData = baseRequest.ProviderData }) as TResponse;
        }

        async Task<TResponse> Retry()
        {
            if (verifyFailCounter < serviceCallMaxTryCounter)
            {
                verifyFailCounter++;
                return await VerifyAsync(baseRequest) as TResponse;
            }
            return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
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
        else
        {
            throw new Exception(error.Error);
        }

        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }
}
