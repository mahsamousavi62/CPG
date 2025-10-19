using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
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

public class AsanPardakhtProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository) : IIpgProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private string userName;
    private string password;
    private int merchantConfigurationId;
    private short[] VerificationSucceededCodes = [200, 472, 473, 475];
    private short[] VerificationVerifyingCodes = [400, 401, 477, 571, 572, 573, 504];
    private short[] VerificationFailedCodes = [471, 474, 476, 478];
    private short[] TransactionResultFetchingCodes = [400, 401, 471, 571, 504];
    private short[] TransactionResultFailedCodes = [472];
    private short[] SettlementSucceededCodes = [200, 474, 476];
    private short[] SettlementFailedCodes = [471, 472, 473, 475, 478];

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

    public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var configViewModel = await _applicationSettingRepositoy.GetAllApplicationSettings();
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
                                                            ServiceTypeId = 1,
                                                            PaymentId = "0",
                                                            CallbackURL = callBack,
                                                            AdditionalData = request.NationalCodeMatchingRequied ? CreateAdditionalData(request.NationalCode, request.ShaparakKey, request.ShaparakIv) : string.Empty,
                                                            MerchantConfigurationId = merchantConfigurationId,
                                                            AmountInRials = (long)request.PaymentRequestAmount,
                                                            LocalInvoiceId = trackerId.ToString(),
                                                        },
                                                        Provider = Enums.ProviderTypeInLog.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtToken,
                                                    }, request, PaymentTokenErrorHandler, (string stringResponse) =>
                                                    {
                                                        var formattedResponse = stringResponse;
                                                        if (!stringResponse.StartsWith("{\"error"))
                                                            formattedResponse = string.Format("{0} {1} {2}", "{\"token\":", stringResponse, "}");

                                                        return System.Text.Json.JsonSerializer.Deserialize<AsanPardakhtTokenResponse>(formattedResponse);
                                                    });
        return new PaymentTokenResponse
        {
            StatusCode = response.StatusCode,
            TrackerId = trackerId.ToString(),
            Token = response.Token,
            IpgBaseUrl = request.IpgBaseUrl,
        };
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
                                                        Provider = Enums.ProviderTypeInLog.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtTransResult,
                                                    }, request, TransactionResultErrorHandler);

        response.Status = response.StatusCode == (short)HttpStatusCode.OK ? (short)2 : response.Status;
        return response;
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = GetHeaders();
        var response = await httpProvider.PostAsync<VerifyTransactionRequest, VerifyTransactionResponse,
                                                    AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        Body = new AsanPardakhtVerifyRequest
                                                        {
                                                            PayGateTranId = request.ProviderTrackerId,
                                                            MerchantConfigurationId = merchantConfigurationId
                                                        },
                                                        BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                        Uri = "v1/Verify",
                                                        HeaderParameters = headers,
                                                        Provider = Enums.ProviderTypeInLog.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtVerify,
                                                    }, request, VerifyErrorHandler, (string stringResponse) =>
                                                    {
                                                        if (string.IsNullOrEmpty(stringResponse))
                                                            return new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.VerificationSucceeded };

                                                        return System.Text.Json.JsonSerializer.Deserialize<VerifyTransactionResponse>(stringResponse);
                                                    });
        return response;
    }

    public async Task<SettleTransactionResponse> Settle(SettleTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var headers = GetHeaders();

        var response = await httpProvider.PostAsync<SettleTransactionRequest, SettleTransactionResponse,
                                                    AsanPardakhtResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        Body = new AsanPardakhtSettleRequest
                                                        {
                                                            PayGateTranId = request.ProviderTrackerId,
                                                            MerchantConfigurationId = merchantConfigurationId
                                                        },
                                                        BaseAddress = "https://ipgrest.asanpardakht.ir/",
                                                        Uri = "v1/Settlement",
                                                        HeaderParameters = headers,
                                                        Provider = Enums.ProviderTypeInLog.AsanPardakht,
                                                        Service = Enums.ServiceType.AsanPardakhtSettle,
                                                    }, request, SettleErrorHandler, (string stringResponse) =>
                                                    {
                                                        if (string.IsNullOrEmpty(stringResponse))
                                                            return new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementSucceeded };

                                                        return System.Text.Json.JsonSerializer.Deserialize<SettleTransactionResponse>(stringResponse);
                                                    });
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

    private Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
      where TResponse : AsanPardakhtTokenResponse
      where TError : AsanPardakhtResponseBase
      where TBaseRequest : PaymentTokenRequest
    {
        return Task.FromResult<TResponse?>(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
    }

    private Task<TResponse?> TransactionResultErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : TransactionResultResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : TransactionResultRequest
    {
        var result = statusCode switch
        {
            _ when statusCode.IsIn(TransactionResultFetchingCodes) => new TransactionResultResponse { Status = 1 } as TResponse,
            _ when statusCode.IsIn(TransactionResultFailedCodes) => new TransactionResultResponse { Status = 3 } as TResponse,
            _ => new TransactionResultResponse { Status = 1 } as TResponse,
        };

        return Task.FromResult<TResponse?>(result);
    }

    private Task<TResponse?> VerifyErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : VerifyTransactionResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : VerifyTransactionRequest
    {
        var result = statusCode switch
        {
            _ when statusCode.IsIn(VerificationVerifyingCodes) => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,
            _ when statusCode.IsIn(VerificationSucceededCodes) => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.VerificationSucceeded } as TResponse,
            _ when statusCode.IsIn(VerificationFailedCodes) => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.VerificationFailed } as TResponse,
            _ => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,
        };

        return Task.FromResult<TResponse?>(result);
    }

    private Task<TResponse?> SettleErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
    where TResponse : SettleTransactionResponse
    where TError : AsanPardakhtResponseBase
    where TBaseRequest : SettleTransactionRequest
    {
        var result = statusCode switch
        {
            _ when statusCode.IsIn(SettlementSucceededCodes) => new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementSucceeded } as TResponse,
            _ when statusCode.IsIn(SettlementFailedCodes) => new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementFailed } as TResponse,
            _ => new SettleTransactionResponse { Status = Enums.IPGTransactionStatus.SettlementFailed } as TResponse,
        };

        return Task.FromResult<TResponse?>(result);
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(AsanPardakhtResponseBase? error)
       where TResponse : AsanPardakhtResponseBase
       where TError : AsanPardakhtResponseBase
       where TBaseRequest : class
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