using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using static System.Net.WebRequestMethods;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class AsanPardakhtProvider : IIpgProvider
    {
        public IHttpClientFactory ClientFactory;
        public IMediator Mediator;
        public IApplicationSettingsRepository ApplicationSettingRepositoy;
        public ReadDbContext Context;

        public async Task<ResultData<PaymentTicketResponse>> GetPaymentTicketAsync(PaymentTicketRequest paymentIpgRequest)
        {
            var companyIpg = await Context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == paymentIpgRequest.CompanyIPGId);

            dynamic jsonObjectProviderData = JObject.Parse(companyIpg.ProviderData);

            ResultData<PaymentTicketResponse> resultData = new();
            var client = ClientFactory.CreateClient("asanpardakhtClient");
            client.DefaultRequestHeaders.Add("usr", (string)jsonObjectProviderData["User_Name"]);
            client.DefaultRequestHeaders.Add("pwd", (string)jsonObjectProviderData["Password"]);
            AsanPardakhtRequest asanPardakhtRequest = await CreateRequestModel(paymentIpgRequest, jsonObjectProviderData);
            string json = JsonConvert.SerializeObject(asanPardakhtRequest);
            var content = new StringContent(json, null, "application/json-patch+json");
            var response = await client.PostAsync("v1/Token", content);
            string result = await response.Content.ReadAsStringAsync();
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    resultData.OperationResult = Enums.OperationResult.Succeeded;
                    Params p = new Params{Token = result.Trim('"')};
                    resultData.Data = new PaymentTicketResponse {  Params=p,Url= "https://asan.shaparak.ir" };
                }
                else
                {
                    dynamic jsonObjectResult = JObject.Parse(result);
                    JToken errorMessage = jsonObjectResult.SelectToken("error.message");
                    resultData.Error = $"{response.StatusCode} - {errorMessage}";
                    resultData.OperationResult = Enums.OperationResult.Failed;
                }
            }
            catch (Exception ex)
            {
                resultData.Error = $"{response.StatusCode} - {ex.Message}";
                resultData.OperationResult = Enums.OperationResult.Failed;
            }
            return resultData;
        }
        private async Task<AsanPardakhtRequest> CreateRequestModel(PaymentTicketRequest paymentIpgRequest, dynamic jsonObjectProviderData)
        {
            var configViewModel = await ApplicationSettingRepositoy.GetAllApplicationSettings();
            var payment = await Context.PaymentRequestReadModels.SingleOrDefaultAsync(p=>p.Id==paymentIpgRequest.PaymentRequestId);

            var model = new AsanPardakhtRequest
            {
                serviceTypeId = 1,
                paymentId = "0",
                //TODO:??
               // https://rhpayment1.br.charisma.ir/p/b/0123456789012345?pcu=https://cpg-stage.charisma.digital/subscription/ipg/result
                callbackURL = $"{configViewModel.IPG_Callback_URL.Trim()}?track_id={payment.TrackerId}",
                additionalData = CreateAdditionalData(jsonObjectProviderData),
                merchantConfigurationId = (int)jsonObjectProviderData["Merchant_Configuration_Id"],
                amountInRials = (long)payment.Amount,
                localInvoiceId= payment.TrackerId
            };

            return model;
        }
        public string CreateAdditionalData(dynamic jsonObjectProviderData)
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

    }
}
