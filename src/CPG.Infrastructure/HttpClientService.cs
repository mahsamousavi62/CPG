using Confluent.Kafka;
using CPG.Application.Shared;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPG.Infrastructure
{
    public class HttpClientFactoryService : IHttpClientFactoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<string> Execute(GetIdpUserProfileModel model)
        {
            var accessToken = await GetClientCredentialsToken(model.Authority, model.ClientId, model.ClientSecret, model.Scope);
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.SetBearerToken( accessToken);
            using var response = await httpClient.GetAsync($"{model.IdpGetProfileUrl}{model.IdpId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        
        private async Task<string> GetClientCredentialsToken(string authority, string clientId, string clientSecret, string scope)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
                Address = authority,
                Policy = { RequireHttps = false } // Todo: RequireHttps = true
            });
            if (disco.IsError)
                return $"Error:{disco.Error}";

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope,
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
                return $"Error:{tokenResponse.Error} ErrorDescription:{tokenResponse.ErrorDescription}";

            return tokenResponse.AccessToken;
        }
    }
}


