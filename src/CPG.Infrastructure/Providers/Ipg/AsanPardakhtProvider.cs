using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class AsanPardakhtProvider(IHttpProvider httpProvider, ReadDbContext context) : IIpgProvider
    {
        public IApplicationSettingsRepository ApplicationSettingRepositoy;
        private readonly IHttpProvider httpProvider = httpProvider;
        private readonly ReadDbContext context = context;

        public async Task<ResultData<PaymentTokenResponse>> GetPaymentTokenAsync(PaymentTokenRequest request)
        {
            dynamic jsonObjectProviderData = JObject.Parse(request.ProviderData);

            ResultData<PaymentTokenResponse> resultData = new();
            var headers = GetHeaders(jsonObjectProviderData);
            var configViewModel = await ApplicationSettingRepositoy.GetAllApplicationSettings();
            var trackerId = await GetTrackerIdAsync();
            var req = new AsanPardakhtTokenRequest
            {
                serviceTypeId = 1,
                paymentId = "0",
                //TODO:??
                // https://rhpayment1.br.charisma.ir/p/b/0123456789012345?pcu=https://cpg-stage.charisma.digital/subscription/ipg/result
                callbackURL = $"{configViewModel.IPG_Callback_URL.Trim()}?track_id={trackerId}",
                additionalData = CreateAdditionalData(jsonObjectProviderData),
                merchantConfigurationId = (int)jsonObjectProviderData["Merchant_Configuration_Id"],
                amountInRials = (long)request.PaymentRequestAmount,
                localInvoiceId = trackerId.ToString(),
            };
            var response = await httpProvider.PostAsync3<AsanPardakhtTokenRequest, AsanPardakhtTokenResponse,
                                                        AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                        {
                                                            BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                            Uri = "v1/Token",
                                                            HeaderParameters = headers,
                                                            Body = req,
                                                            Provider = Enums.ProviderType.AsanPardakht,
                                                            Service = Enums.ServiceType.AsanPardakhtToken,
                                                        }, req, PaymentTokenErrorHandler, (string stringResponse) =>
                                                        {
                                                            var formattedResponse = stringResponse;
                                                            if (!stringResponse.StartsWith("{\"error"))
                                                            {
                                                                formattedResponse = string.Format("{0} {1} {2}" ,"{\"token\":", stringResponse, "}");
                                                            }
                                                            return System.Text.Json.JsonSerializer.Deserialize<AsanPardakhtTokenResponse>(formattedResponse);
                                                        });

            Params p = new Params { Token = response.Token };
            return new ResultData<PaymentTokenResponse>
            {
                OperationResult = Enums.OperationResult.Succeeded,
                Data = new PaymentTokenResponse { Params = p, Url = "https://asan.shaparak.ir", TrackerId = trackerId.ToString() },                
            };
        }

        
        private string CreateAdditionalData(dynamic jsonObjectProviderData)
        {
            string hexString = Guid.NewGuid().ToString("N");
            string randomString = hexString.Substring(0, 7);
            //Todo:remove hardcode!
            var original = $"0|0440061423|{randomString}";
            string key = (string)jsonObjectProviderData["Shaparak_Tabesh_Key"];
            string iv = (string)jsonObjectProviderData["Shaparak_Tabesh_IV"];

            var dkey = AesHelper.Base64Decode(key);
            var div = AesHelper.Base64Decode(iv);
            var token = AesHelper.EncryptAes(original, dkey ?? string.Empty, div ?? string.Empty);
            var json = JsonConvert.SerializeObject(new { EncryptedNationalId = token });
            return json;
        }

        private List<(string Key, string? Value)> GetHeaders(dynamic providerData)
        {

            var list = new List<(string Key, string? Value)> { ("usr", (string)providerData["User_Name"]),
                                                               ("pwd", (string)providerData["Password"]),
                                                               ("accept", "text/plain")
            };
            return list;
        }

        private async Task<long> GetTrackerIdAsync()
        {
            try
            {
                return await context.GetNextSequenceValue();
            }
            catch (Exception ex)
            {
                //logger.Value.LogError(ex.Message, ex, nameof(GetBatchIdAsync));
                throw;
            }
        }

        public async Task<ResultData<TransactionResultResponse>> GetTransactionResult(TransactionResultRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //var client = ClientFactory.CreateClient("charisPayClient");
            return await Task.FromResult(new ResultData<TransactionResultResponse>());
        }

        private static async Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error)
          where TResponse : AsanPardakhtTokenResponse
          where TError : AsanPardakhtResponseBase
          where TBaseRequest : AsanPardakhtTokenRequest
        {
            if (error?.ErrorResult is not null)
            {
                throw new Exception(error.ErrorResult.Message);
            }
            else
            {
                return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
            }
        }

        private static TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(AsanPardakhtResponseBase? error)
           where TResponse : AsanPardakhtResponseBase
           where TError : AsanPardakhtResponseBase
           where TBaseRequest : AsanPardakhtRequestBase

        {
            throw new Exception(error?.ErrorResult.Message);
        }
    }
}
