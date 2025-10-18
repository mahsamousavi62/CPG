using Azure.Core;
using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay;
using CPG.Domain.SharedKernel.Communication.Charispay.Models;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Providers.NeoBank;
using HotChocolate.Execution.Processing;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly ILogService _logService;
        private readonly ICurrentUser _currentUser;

        public CharisPayProvider(IHttpClientFactory factory, IConfiguration configuration, ILogService logService, ICurrentUser currentUser)
        {
            _factory = factory;
            this.configuration = configuration;
            _logService = logService;
            _currentUser = currentUser;
        }

        public async Task<Result<AccountNumberResponse>> GetAccountNumber(string iban)
        {
            var charisPayConfig = configuration.GetSection("Infrastructure:CharisPay").Get<CharisPayConfig>();
            ResultData<AccountNumberResponse> resultData = new();

            // Serialize and mask request before try block for error logging
            string json = JsonConvert.SerializeObject(new { iban });
            string maskedJson = json;
            if (!string.IsNullOrEmpty(maskedJson))
            {
                maskedJson = System.Text.RegularExpressions.Regex.Replace(maskedJson, Constants.Pattern, Constants.Replaceformat);
            }

            try
            {
                var client = _factory.CreateClient("charisPayClient");
                client.SetBearerToken(charisPayConfig.Token);
                client.DefaultRequestHeaders.Add("correlation-id", Guid.NewGuid().ToString());
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(charisPayConfig.InqueryIbanUrl, content);

                var resultContent = await result.Content.ReadAsStringAsync();

                if (resultContent.StartsWith("{\"result\":\""))
                {
                    resultContent = resultContent.Replace("{\"result\":\"", "{\"errorResult\":\"");
                }
                var callLog = CallLogModel.CreateSuccess(
                    serviceName: "GetAccountNumber",
                    providerName: "CharisPay",
                    requestUri: charisPayConfig.InqueryIbanUrl,
                    requestBody: json,
                    responseBody: resultContent,
                    serviceType: Enums.ServiceType.GetAccountNumber,
                    providerType: Enums.ProviderTypeInLog.CharisPay,
                    auditType: Enums.AuditType.Provider,
                    userId: _currentUser.UserId,
                    responseStatusCode: (int)result.StatusCode
                );

                _logService.LogInformation(callLog);


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

                if (!string.IsNullOrEmpty(response.ErrorResult))
                {
                    resultData.Error = response.ErrorResult;
                    return Result<AccountNumberResponse>.Failure(new Error("2201000", resultData.Error));
                }
                else
                {
                    resultData.Data = new AccountNumberResponse(response.result[0].depositNumber, response.result[0].bankName);
                    return Result<AccountNumberResponse>.SuccessResult(resultData.Data);
                }
            }
            catch (HttpRequestException ex)
            {
                var callLog = CallLogModel.CreateError(
                    serviceName: "GetAccountNumber",
                    providerName: "CharisPay",
                    requestUri: charisPayConfig.InqueryIbanUrl,
                    requestBody: maskedJson,
                    responseBody: ex.Message,
                    exception: ex,
                    serviceType: Enums.ServiceType.GetAccountNumber,
                    providerType: Enums.ProviderTypeInLog.CharisPay,
                    auditType: Enums.AuditType.Provider,
                    userId: _currentUser.UserId
                );
                _logService.LogError(callLog);
                return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }

            catch (Exception ex)
            {
                var callLog = CallLogModel.CreateError(
                    serviceName: "GetAccountNumber",
                    providerName: "CharisPay",
                    requestUri: charisPayConfig.InqueryIbanUrl,
                    requestBody: maskedJson,
                    responseBody: ex.Message,
                    exception: ex,
                    serviceType: Enums.ServiceType.GetAccountNumber,
                    providerType: Enums.ProviderTypeInLog.CharisPay,
                    auditType: Enums.AuditType.Provider,
                    userId: _currentUser.UserId
                );
                _logService.LogError(callLog);
                return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }

        }
    }

}
