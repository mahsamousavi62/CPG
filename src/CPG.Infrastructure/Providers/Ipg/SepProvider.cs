using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Communication.Ipg.Sep;
using CPG.Domain.SharedKernel.Helper;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.Ipg;

public class SepProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository) : IIpgProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private int terminalId;

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            terminalId = jsonObjectProviderData["Terminal_ID"] is not null ? (int)jsonObjectProviderData["Terminal_ID"] : throw new Exception("Invalid TerminalId");
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
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        string callBack = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress, trackerId.ToString(), configViewModel.CPG_BackEnd);

        var response = await httpProvider.PostAsync<PaymentTokenRequest, SepTokenResponse,
                                                    SepResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        BaseAddress = "https://sep.shaparak.ir/",
                                                        Uri = "OnlinePG/OnlinePG",
                                                        Body = new SepTokenRequest
                                                        {
                                                            Action = "token",
                                                            TerminalId = terminalId,
                                                            RedirectUrl = callBack,
                                                            ResNum = trackerId,
                                                            Amount = request.PaymentRequestAmount,
                                                            CellNumber = request.MobileNumber,
                                                            ShaparakKycParams = request.NationalCodeMatchingRequied ? new ShaparakKycParams { CardHolderNationalId = CreateAdditionalData(request.NationalCode, request.ShaparakKey, request.ShaparakIv), ThirdPartyCode = request.ThirdPartyCode.ToString() } : null,
                                                        },
                                                        Provider = Enums.ProviderType.Sep,
                                                        Service = Enums.ServiceType.SepToken,
                                                    }, request, PaymentTokenErrorHandler);

        return new PaymentTokenResponse
        {
            StatusCode = response.StatusCode,
            TrackerId = trackerId,
            Token = response.Token,
            IpgBaseUrl = request.IpgBaseUrl,            
        };  
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new System.NotImplementedException();
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var response = await httpProvider.PostAsync<VerifyTransactionRequest, SepVerifyTransactionResponse,
                                                    SepResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        Body = new SepVerifyTransactionRequest
                                                        {
                                                            RefNum = request.ProviderTrackerId,
                                                            TerminalNumber = terminalId.ToString(),
                                                        },
                                                        BaseAddress = "https://sep.shaparak.ir/",
                                                        Uri = "verifyTxnRandomSessionkey/ipg/VerifyTranscation",                                                        
                                                        Provider = Enums.ProviderType.Sep,
                                                        Service = Enums.ServiceType.SepVerify,
                                                    }, request, VerifyErrorHandler);

        var status = response.ResultCode switch
        {
            -105 or -104 or -106 => Enums.IPGTransactionStatus.Verifying,
            0 or 2 => Enums.IPGTransactionStatus.VerificationSucceeded,
            -2 or -6 or 5 => Enums.IPGTransactionStatus.VerificationFailed,
        };
        return new VerifyTransactionResponse
        {
            Status = status
        };
    }

    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{callbackPage}?track_id={trackerId}",
        2 => $"{siteAddress}/p/b/{trackerId}?pcu={callbackPage.TrimEnd()}/IPGResult",
        _ => string.Empty,
    };

    private string CreateAdditionalData(string nationalCode, string key, string iv)
    {
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 7);
        var original = $"0|{nationalCode}|{randomString}";
        var dkey = AesHelper.Base64Decode(key);
        var div = AesHelper.Base64Decode(iv);
        var token = AesHelper.EncryptAes(original, dkey ?? string.Empty, div ?? string.Empty);        
        return token;
    }

    private async Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
      where TResponse : SepTokenResponse
      where TError : SepResponseBase
      where TBaseRequest : PaymentTokenRequest
    {
        if (tokenFailCounter < serviceCallMaxTryCounter)
        {
            tokenFailCounter++;
            return await GetPaymentTokenAsync(baseRequest) as TResponse;
        }
        return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
    }

    private async Task<TResponse?> VerifyErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
      where TResponse : SepVerifyTransactionResponse
      where TError : SepResponseBase
      where TBaseRequest : VerifyTransactionRequest
    {
        return statusCode switch
        {
            504 => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,           
            _ => verifyFailCounter < serviceCallMaxTryCounter ? await Retry() : await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error)),
        };

        async Task<TResponse> Retry()
        {
            verifyFailCounter++;
            return await Verify(baseRequest) as TResponse;
        }
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(SepResponseBase? error)
       where TResponse : ResponseBase
       where TError : SepResponseBase
       where TBaseRequest : class

    {
        if (error is null || error.ErrorCode is null)
        {
            throw new Exception(GlobalResource.ProviderUnexpectedError);
        }
        if (!string.IsNullOrEmpty(error.ErrorDescription))
        {
            throw new Exception($"ServiceProviderError: {error.ErrorDescription}");
        }

        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }
}
