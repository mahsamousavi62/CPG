using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Confluent.Kafka;
using System.Reflection.Metadata;
using CPG.Domain.SharedKernel.ApplicationSettings;
using Microsoft.Identity.Client;
using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.SharedKernel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using CPG.Domain.SharedKernel.Communication.Idp;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;

namespace CPG.Infrastructure.Providers.Idp;
public class IdpProvider(IHttpClientFactory httpClientFactory, IApplicationSettingsRepository applicationSettingsRepository) : IIdpProvider
{
    public readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;

    public async Task<ResultData<UserProfileResponse>> GetUserProfile(string idpId)
    {
        ResultData<UserProfileResponse> resultData = new();

        var appConfig = await _applicationSettingsRepository.GetAllApplicationSettings();

        var accessTokenResult = await GetClientCredentialsToken(appConfig);
        if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
            return new ResultData<UserProfileResponse> { Error = accessTokenResult.Error, OperationResult = Enums.OperationResult.Failed };

        var client = _httpClientFactory.CreateClient("idpClient");
        client.SetBearerToken(accessTokenResult.Data);
        using var response = await client.GetAsync($"{appConfig.IdpGetProfileUrl}{idpId}");
        if (!response.IsSuccessStatusCode)
            return new ResultData<UserProfileResponse>
            {
                Error = response.StatusCode.ToString(),
                OperationResult = Enums.OperationResult.Failed
            };
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        try
        {
            resultData.Data = JsonConvert.DeserializeObject<UserProfileResponse>(content);
            resultData.OperationResult = Enums.OperationResult.Succeeded;
        }
        catch (Exception)
        {
            dynamic d = JObject.Parse(content);

            resultData.Error = d;
            resultData.OperationResult = Enums.OperationResult.Failed;
        }
        return resultData;
    }

    private async Task<ResultData<string>> GetClientCredentialsToken(ApplicationConfigViewModel appConfig)
    {
        var httpClient = _httpClientFactory.CreateClient("idpClient");

        var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = appConfig.Authority,
            Policy = { RequireHttps = false } // Todo: RequireHttps = true
        });
        if (disco.IsError)
            return new ResultData<string> { Error = disco.Error, OperationResult = Enums.OperationResult.Failed };

        var tokenRequest = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = appConfig.ServerApiKey,
            ClientSecret = appConfig.ServerApiSecret,
            Scope = appConfig.ServerScope,
        };
        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

        if (tokenResponse.IsError)
            return new ResultData<string>
            {
                Error = $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}"
                                           ,
                OperationResult = Enums.OperationResult.Failed
            };

        return new ResultData<string>
        {
            Data = tokenResponse.AccessToken,
            OperationResult = Enums.OperationResult.Succeeded
        };
    }
}

