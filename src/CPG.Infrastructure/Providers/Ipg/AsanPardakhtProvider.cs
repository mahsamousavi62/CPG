using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CommunityToolkit.HighPerformance;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CPG.Infrastructure.Providers.Ipg;

public class AsanPardakhtProvider(IHttpProvider httpProvider, ReadDbContext context) : IIpgProvider
{
    public IApplicationSettingsRepository ApplicationSettingRepositoy;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private string userName;
    private string password;
    private int merchantConfigurationId;
    private string key;
    private string iv;
    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            merchantConfigurationId = jsonObjectProviderData["Merchant_Configuration_Id"] is not null? (int)jsonObjectProviderData["Merchant_Configuration_Id"] : throw new Exception("Invalid merchantConfigurationId");
            userName = jsonObjectProviderData["User_Name"] is not null ? (string)jsonObjectProviderData["User_Name"] : throw new Exception("Invalid User_Name");
            password = jsonObjectProviderData["Password"] is not null ? (string)jsonObjectProviderData["Password"] : throw new Exception("Invalid Password");
            key = jsonObjectProviderData["Shaparak_Tabesh_Key"] is not null ? (string)jsonObjectProviderData["Shaparak_Tabesh_Key"] : throw new Exception("Invalid Shaparak_Tabesh_Key");
            iv = jsonObjectProviderData["Shaparak_Tabesh_IV"] is not null ? (string)jsonObjectProviderData["Shaparak_Tabesh_IV"] : throw new Exception("Invalid Shaparak_Tabesh_IV");
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }
    public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var configViewModel = await ApplicationSettingRepositoy.GetAllApplicationSettings();

        ResultData<PaymentTokenResponse> resultData = new();
        var headers = GetHeaders();
        var trackerId = await GetTrackerIdAsync();
        var callBack = await CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress, trackerId.ToString(), configViewModel.CPG_BackEnd);
        var req = new AsanPardakhtTokenRequest
        {
            serviceTypeId = 1,
            paymentId = "0",
            callbackURL = callBack,
            additionalData = CreateAdditionalData(),
            merchantConfigurationId = merchantConfigurationId,
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
                                                            formattedResponse = string.Format("{0} {1} {2}", "{\"token\":", stringResponse, "}");
                                                       
                                                        return System.Text.Json.JsonSerializer.Deserialize<AsanPardakhtTokenResponse>(formattedResponse);
                                                    });

        return new PaymentTokenResponse
        {
            Url = $"{request.SiteAddress}/redirectToBank"
           ,
            TrackerId = trackerId.ToString(),
            JsonBody = new UrlResponseModel
            {
                Params = new Params { Token = response.Token },
                Url = "https://asan.shaparak.ir",
            }
        };
}

public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest request)
{
    dynamic jsonObjectProviderData = JObject.Parse(request.ProviderData);
    ResultData<TransactionResultResponse> resultData = new();
    var headers = GetHeaders();
    var response = await httpProvider.GetAsync<TransactionResultRequest, TransactionResultResponse,
                                                AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                {
                                                    Body = new
                                                    {
                                                        LocalInvoiceId = request.LocalInvoiceId,
                                                        MerchantConfigurationId = merchantConfigurationId
                                                    },
                                                    BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                    Uri = "v1/TranResult",
                                                    HeaderParameters = headers,
                                                    Provider = Enums.ProviderType.AsanPardakht,
                                                    Service = Enums.ServiceType.AsanPardakhtTransResult,
                                                }, request, TransactionResultErrorHandler);

    return response;
}

private string CreateAdditionalData()
{
    string hexString = Guid.NewGuid().ToString("N");
    string randomString = hexString.Substring(0, 7);
    //Todo:remove hardcode!
    var original = $"0|0440061423|{randomString}";
    var dkey = AesHelper.Base64Decode(key);
    var div = AesHelper.Base64Decode(iv);
    var token = AesHelper.EncryptAes(original, dkey ?? string.Empty, div ?? string.Empty);
    var json = JsonConvert.SerializeObject(new { EncryptedNationalId = token });
    return json;
}

private List<(string Key, string? Value)> GetHeaders()
{
    var list = new List<(string Key, string? Value)> { ("usr", userName), ("pwd", password), ("accept", "text/plain") };
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
        throw ex;
    }
}

private async Task<string> CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage)
{

    switch (ipgRedirectionType)
    {
        case 1:
            return $"{siteAddress}/{callbackPage}?track_id={trackerId}";
        case 2:
            return $"{siteAddress}/p/b/{trackerId}?pcu={callbackPage.TrimEnd()}/IPGResult";
        default:
            return string.Empty;
    }
    ;
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

private static async Task<TResponse?> TransactionResultErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error)
where TResponse : TransactionResultResponse
where TError : AsanPardakhtResponseBase
where TBaseRequest : TransactionResultRequest
{
    if (error?.ErrorResult is not null)
    {
        switch (error.ErrorResult.Code)
        {
            case 1043:
                throw new Exception("InvalidOrExpiredInvoiceId");
            default:
                break;
        }
        throw new Exception(error.ErrorResult.Message);
    }
    else
    {
        return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
    }
}

private static async Task<TResponse?> PaymentTransactionErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error)
where TResponse : TransactionResultResponse
where TError : AsanPardakhtResponseBase
where TBaseRequest : TransactionResultRequest
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
    if (error is null)
    {
        throw new Exception("UnknownError");
    }
    if (error.ErrorResult is null)
    {
        throw new Exception("UnknownError");
    }
    if (!string.IsNullOrEmpty(error.ErrorResult.Message))
    {
        throw new Exception($"ServiceProviderError: {error.ErrorResult.Message}");
    }

    throw new Exception("ServiceProviderError");
}
}