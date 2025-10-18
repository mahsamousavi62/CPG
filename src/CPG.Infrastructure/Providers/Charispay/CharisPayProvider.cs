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
            try
            {
                var client = _factory.CreateClient("charisPayClient");
                client.SetBearerToken(charisPayConfig.Token);
                client.DefaultRequestHeaders.Add("correlation-id", Guid.NewGuid().ToString());
                string json = JsonConvert.SerializeObject(new { iban });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(charisPayConfig.InqueryIbanUrl, content);

                var resultContent = await result.Content.ReadAsStringAsync();

                if (resultContent.StartsWith("{\"result\":\""))
                {
                    resultContent = resultContent.Replace("{\"result\":\"", "{\"errorResult\":\"");
                }
                var callLog = new CallLogModel
                {
                    RequestBody = json,
                    ResponseBody = resultContent,
                    ServiceCallDate = DateTime.Now,
                    ServiceCallUrl = charisPayConfig.InqueryIbanUrl,
                    ServiceCallStatus = result.StatusCode == System.Net.HttpStatusCode.OK,
                    ServiceType = Enums.ServiceType.GetAccountNumber,
                    CreationDate = DateTime.Now,
                    CreationUserId = _currentUser.UserId,
                    ProviderType = Enums.ProviderTypeInLog.CharisPay,
                    AuditType = Enums.AuditType.Provider
                };

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
                var callLog = new CallLogModel
                {
                    RequestBody = JsonConvert.SerializeObject(new { iban }),
                    ResponseBody = ex.Message,
                    ServiceCallDate = DateTime.Now,
                    ServiceCallUrl = charisPayConfig.InqueryIbanUrl,
                    ServiceCallStatus = false,
                    ServiceType = Enums.ServiceType.GetAccountNumber,
                    CreationDate = DateTime.Now,
                    CreationUserId = _currentUser.UserId,
                    ErrorCode = ex.GetType().Name,
                    ErrorType = ex.Message,
                    ProviderType = Enums.ProviderTypeInLog.CharisPay,
                    AuditType = Enums.AuditType.Provider
                };
                _logService.LogError(callLog);
                return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }

            catch (Exception ex)
            {
                var callLog = new CallLogModel
                {
                    RequestBody = JsonConvert.SerializeObject(new { iban }),
                    ResponseBody = ex.Message,
                    ServiceCallDate = DateTime.Now,
                    ServiceCallUrl = charisPayConfig.InqueryIbanUrl,
                    ServiceCallStatus = false,
                    ServiceType = Enums.ServiceType.GetAccountNumber,
                    CreationDate = DateTime.Now,
                    CreationUserId = _currentUser.UserId,
                    ErrorCode = ex.GetType().Name,
                    ErrorType = ex.Message,
                    ProviderType = Enums.ProviderTypeInLog.CharisPay,
                    AuditType = Enums.AuditType.Provider
                };
                _logService.LogError(callLog);
                return Result<AccountNumberResponse>.Failure(new Error("2201000", GlobalResource.UnexpectedError));
            }

        }
    }

}
