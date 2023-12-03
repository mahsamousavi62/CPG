using Azure.Core;
using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ClientFactory;
using HotChocolate.Execution.Processing;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task< ResultData<AccountNumberViewModel>> GetAccountNumber(string iban)
        {
            ResultData<AccountNumberViewModel> resultData = new();
            var client = _factory.CreateClient("charisPayClient");
            client.SetBearerToken(configuration["Infrastructure:CharisPay:Token"]);
            client.DefaultRequestHeaders.Add("correlation-id", Guid.NewGuid().ToString());
            string json = JsonConvert.SerializeObject(new { iban });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(configuration["Infrastructure:CharisPay:InqueryIbanUrl"], content);

            var resultContent = await result.Content.ReadAsStringAsync();
            try
            {
            var response = JsonConvert.DeserializeObject<inqueryIbanViewModel>(resultContent);
                resultData.Data= new AccountNumberViewModel(response.result[0].depositNumber, response.result[0].bankName);
                resultData.OperationResult = Enums.OperationResult.Succeeded;
            }
            catch (Exception)
            {
                dynamic d = JObject.Parse(resultContent);

                resultData.Error= d.result;
                resultData.OperationResult = Enums.OperationResult.Failed;
            }
            return resultData;
        }
    }

}
