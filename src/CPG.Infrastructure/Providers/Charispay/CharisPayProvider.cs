using Azure.Core;
using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay;
using CPG.Domain.SharedKernel.Communication.Charispay.Models;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Providers.NeoBank;
using HotChocolate.Execution.Processing;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Context;
using Serilog.Core;
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
        private readonly ILogger<CharisPayProvider> _logger;


        public CharisPayProvider(IHttpClientFactory factory, IConfiguration configuration, ILogger<CharisPayProvider> logger)
        {
            _factory = factory;
            this.configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<AccountNumberResponse>> GetAccountNumber(string iban)
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
                var callLog = new CallLogModel
                {
                    RequestBody = json,
                    ResponseBody = resultContent,
                    ServiceCallDate = DateTime.Now,
                    ServiceCallUrl = charisPayConfig.InqueryIbanUrl,
                    ServiceCallStatus = result.StatusCode == System.Net.HttpStatusCode.OK,
                    ServiceType = Enums.ServiceType.GetAccountNumber,
                    CreationDate = DateTime.Now,
                    CreationUserId = 1,
                    ProviderType = Enums.ProviderType.NeoBank,
                    AuditType = Enums.AuditType.Provider
                };

                using (LogContext.PushProperty("CallLog", callLog, true))
                {
                    _logger.LogInformation("[CallLog] {@CallLog}", callLog);
                }

                var response = JsonConvert.DeserializeObject<CharispayResponseBase<InquiryIbanResponse>>(resultContent);
              
                if (response == null)
                {
                    return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.NotHasResponse));
                }
                if (response.unAuthorizedRequest)
                {
                    return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnAuthorizeRequest));

                }
                if (response.error != null || !response.success)
                {
                    return Result<AccountNumberResponse>.Failure(new Error("2201000", response.error.ToString()));
                }
                resultData.Data = new AccountNumberResponse(response.result[0].depositNumber, response.result[0].bankName);
                return Result<AccountNumberResponse>.SuccessResult(resultData.Data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Request: Unhandled Exception for Request {nameof(GetAccountNumber)}{ex.Message} ");
                return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }

        }
    }

}
