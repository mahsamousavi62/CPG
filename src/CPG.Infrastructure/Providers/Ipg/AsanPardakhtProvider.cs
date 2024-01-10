using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Helper;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CPG.Infrastructure.Providers.Ipg;

public class AsanPardakhtProvider(IHttpProvider httpProvider, ReadDbContext context) : IIpgProvider
{
    public IApplicationSettingsRepository ApplicationSettingRepositoy;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private string userName;
    private string password;
    private int merchantConfigurationId; 

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            merchantConfigurationId = jsonObjectProviderData["Merchant_Configuration_Id"] is not null ? (int)jsonObjectProviderData["Merchant_Configuration_Id"] : throw new Exception("Invalid merchantConfigurationId");
            userName = jsonObjectProviderData["User_Name"] is not null ? (string)jsonObjectProviderData["User_Name"] : throw new Exception("Invalid User_Name");
            password = jsonObjectProviderData["Password"] is not null ? (string)jsonObjectProviderData["Password"] : throw new Exception("Invalid Password");            
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }

    public async Task<(short, PaymentTokenResponse)> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var configViewModel = await ApplicationSettingRepositoy.GetAllApplicationSettings();
        var headers = GetHeaders();
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        string callBack = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress, trackerId.ToString(), configViewModel.CPG_BackEnd);
       
        var response = await httpProvider.PostAsync3<PaymentTokenRequest, AsanPardakhtTokenResponse,
                                                    AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                        Uri = "v1/Token",
                                                        HeaderParameters = headers,
                                                        Body = new AsanPardakhtTokenRequest
                                                        {
                                                            serviceTypeId = 1,
                                                            paymentId = "0",
                                                            callbackURL = callBack,
                                                            additionalData = request.NationalCodeMatchingRequied ? CreateAdditionalData(request.NationalCode , request.ShaparakKey, request.ShaparakIv) : string.Empty,
                                                            merchantConfigurationId = merchantConfigurationId,
                                                            amountInRials = (long)request.PaymentRequestAmount,
                                                            localInvoiceId = trackerId.ToString(),
                                                        },
                                                        Provider = Enums.ProviderType.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtToken,
                                                    }, request, PaymentTokenErrorHandler, (string stringResponse) =>
                                                    {
                                                        var formattedResponse = stringResponse;
                                                        if (!stringResponse.StartsWith("{\"error"))
                                                            formattedResponse = string.Format("{0} {1} {2}", "{\"token\":", stringResponse, "}");

                                                        return System.Text.Json.JsonSerializer.Deserialize<AsanPardakhtTokenResponse>(formattedResponse);
                                                    });

        var paymentToken = new PaymentTokenResponse
        {
            Url = $"{request.SiteAddress}/redirectToBank",
            TrackerId = trackerId.ToString(),
            JsonBody = new JsonStrModel
            {
                JsonStr = new()
                {
                    Params = new Params { RefID = response.Token, MobileAp = request.MobileNumber },
                    Url = request.IpgBaseUrl,
                    
                }
            }
        };

        return (response.Status, paymentToken);
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
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
        response.Status = response.Status == 200 ? (short)2 : response.Status;
        return response;
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        ResultData<VerifyTransactionResponse> resultData = new();
        var headers = GetHeaders();
        var response = await httpProvider.PostAsync<VerifyTransactionRequest, VerifyTransactionResponse,
                                                    AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        Body = new VerifyRequest
                                                        {
                                                            PayGateTranId = request.ProviderTrackerId,
                                                            MerchantConfigurationId = merchantConfigurationId
                                                        },
                                                        BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                        Uri = "v1/Verify",
                                                        HeaderParameters = headers,
                                                        Provider = Enums.ProviderType.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtTransResult,
                                                    }, request, VerifyErrorHandler);

        return response;
    }

    private string CreateAdditionalData(string nationalCode, string key, string iv)
    {        
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 7);
        var original = $"0|{nationalCode}|{randomString}";
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

    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{callbackPage}?track_id={trackerId}",
        2 => $"{siteAddress}/p/b/{trackerId}?pcu={callbackPage.TrimEnd()}/IPGResult",
        _ => string.Empty,
    };

    private async Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
      where TResponse : AsanPardakhtTokenResponse
      where TError : AsanPardakhtResponseBase
      where TBaseRequest : PaymentTokenRequest
    {
        if (tokenFailCounter < serviceCallMaxTryCounter)
        {
            tokenFailCounter++;
            return await GetPaymentTokenAsync(baseRequest) as TResponse;
        }
        return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
    }

    private async Task<TResponse?> TransactionResultErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : TransactionResultResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : TransactionResultRequest
    {
        return statusCode switch
        {
            400 or 401 or 471 or 571 or 504 => transactionResultFailCounter < serviceCallMaxTryCounter ? await Retry() : new TransactionResultResponse { Status = 1 } as TResponse,
            472 => new TransactionResultResponse { Status = 3 } as TResponse,
            _ => new TransactionResultResponse { Status = 1 } as TResponse,
        };

        async Task<TResponse> Retry()
        {
            transactionResultFailCounter++;
            return await GetTransactionResult(baseRequest) as TResponse;
        }
    }

    private async Task<TResponse?> VerifyErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : VerifyTransactionResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : VerifyTransactionRequest
    {
        return statusCode switch
        {
            400 or 401 or 477 or 571 or 572 or 573 or 504 => verifyFailCounter < serviceCallMaxTryCounter ? await Retry() : new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,
            200 or 472 or 473 or 475 => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.VerificationSucceeded } as TResponse,
            471 or 474 or 476 or 478 => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.VerificationFailed } as TResponse,
            _ => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,
        };

        async Task<TResponse> Retry()
        {
            verifyFailCounter++;
            return await Verify(baseRequest) as TResponse;
        }
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(AsanPardakhtResponseBase? error)
       where TResponse : AsanPardakhtResponseBase
       where TError : AsanPardakhtResponseBase
       where TBaseRequest : AsanPardakhtRequestBase

    {
        if (error is null || error.ErrorResult is null)
        {
            throw new Exception(GlobalResource.ProviderUnexpectedError);
        }
        if (!string.IsNullOrEmpty(error.ErrorResult.Message))
        {
            throw new Exception($"ServiceProviderError: {error.ErrorResult.Message}");
        }

        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }
}