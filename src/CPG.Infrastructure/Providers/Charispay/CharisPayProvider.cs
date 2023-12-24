using Azure.Core;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay;
using CPG.Domain.SharedKernel.Communication.Charispay.Models;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
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

namespace CPG.Infrastructure.Providers.Charispay
{
    public class CharisPayProvider : ICharisPayProvider
    {
        public readonly IHttpClientFactory _factory;
        private readonly IConfiguration configuration;

        public CharisPayProvider(IHttpClientFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            this.configuration = configuration;
        }

        public async Task<ResultData<AccountNumberResponse>> GetAccountNumber(string iban)
        {
            var charisPayConfig = configuration.GetSection("Infrastructure:CharisPay").Get<CharisPayConfig>();
                        ResultData<AccountNumberResponse> resultData = new();
            
                var client = _factory.CreateClient("charisPayClient");
                client.SetBearerToken(charisPayConfig.Token);
                client.DefaultRequestHeaders.Add("correlation-id", Guid.NewGuid().ToString());
                string json = JsonConvert.SerializeObject(new { iban });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(charisPayConfig.InqueryIbanUrl, content);

                var resultContent = await result.Content.ReadAsStringAsync();
                try
                {
                    var response = JsonConvert.DeserializeObject<CharispayResponseBase<InquiryIbanResponse>>(resultContent);
                    resultData.Data = new AccountNumberResponse(response.result[0].depositNumber, response.result[0].bankName);
                    resultData.OperationResult = Enums.OperationResult.Succeeded;
                }
                catch (Exception)
                {
                    dynamic d = JObject.Parse(resultContent);

                    resultData.Error = d.result;
                    resultData.OperationResult = Enums.OperationResult.Failed;
                }
            return resultData;
        }
    }

}
