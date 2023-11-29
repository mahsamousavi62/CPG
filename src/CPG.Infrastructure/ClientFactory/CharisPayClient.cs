using Azure.Core;
using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.SharedKernel.ClientFactory;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Juros.Infrastructure.External
{
    public class CharisPayClient : ICharisPayClient
    {
        public readonly IHttpClientFactory _factory;
        private readonly IConfiguration configuration;

        public CharisPayClient(IHttpClientFactory factory,IConfiguration configuration)
        {
            _factory = factory;
            this.configuration = configuration;
        }

        public async Task<AccountNumberViewModel> GetAccountNumber(string iban)
        {
            var client = _factory.CreateClient("charisPayClient");
            client.SetBearerToken(configuration["Infrastructure:CharisPay:Token"]);
            client.DefaultRequestHeaders.Add("correlation-id", Guid.NewGuid().ToString());
            string json = JsonConvert.SerializeObject(new { iban });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(configuration["Infrastructure:CharisPay:InqueryIbanUrl"], content);

            var resultContent = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<inqueryIbanViewModel>(resultContent);

            return new AccountNumberViewModel(response.result[0].depositNumber);
        }
    }

}
