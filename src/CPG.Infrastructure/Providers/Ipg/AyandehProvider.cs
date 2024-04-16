using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Ayandeh;
using System.Globalization;
using System.Collections.Generic;
using ServiceAmount = CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentToken.ServiceAmount;

namespace CPG.Infrastructure.Providers.Ipg;

internal class AyandehProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository) : IIpgProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private string serviceId;
    private string userName;
    private string password;

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            serviceId = jsonObjectProviderData["Service_ID"] is not null ? jsonObjectProviderData["Service_ID"] : throw new Exception("Invalid Service_ID");
            userName = jsonObjectProviderData["User_Name"] is not null ? jsonObjectProviderData["User_Name"] : throw new Exception("Invalid User_Name");
            password = jsonObjectProviderData["Password"] is not null ? jsonObjectProviderData["Password"] : throw new Exception("Invalid Password");
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
        var date = DateTime.Now.AddDays(1);
        var pc = new PersianCalendar();
        var settleDate = string.Format("{0}{1}{2}", pc.GetYear(date), pc.GetMonth(date).ToString("D2"), pc.GetDayOfMonth(date).ToString("D2"));

        var response = await httpProvider.PostAsync<PaymentTokenRequest, AyandehTokenResponse,
                                                    AyandehResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        BaseAddress = "https://mpg.ba24.ir/",
                                                        Uri = "mpg/api/ipgGetTraceId",
                                                        Body = new AyandehTokenRequest
                                                        {
                                                            Username = userName,
                                                            Password = password,
                                                            AdditionalData = $"{request.NationalCode}-{request.PaymentId}",
                                                            CallBackUrl = callBack,
                                                            Amount = request.PaymentRequestAmount.ToString(),
                                                            ServiceAmountList = new List<ServiceAmount> {
                                                                new ServiceAmount
                                                                {
                                                                    ServiceId = serviceId,
                                                                    Amount = request.PaymentRequestAmount.ToString()
                                                                }
                                                            },                                                            
                                                            Mobile = !string.IsNullOrEmpty(request.MobileNumber) ? $"{request.MobileNumber.Remove(0, 1)}" : null,
                                                            SettleDate = settleDate,
                                                        },
                                                        Provider = Enums.ProviderType.Ayandeh,
                                                        Service = Enums.ServiceType.AyandehToken,
                                                    }, request, PaymentTokenErrorHandler);

        return new PaymentTokenResponse
        {
            StatusCode = response.StatusCode,            
            TrackerId = trackerId,
            Token = response.TraceNumber,
            IpgBaseUrl = request.IpgBaseUrl,
            Result = response.Result,
            UserName = userName,
        };
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var response = await httpProvider.PostAsync<VerifyTransactionRequest, AyandehVerifyTransactionResponse,
                                                    AyandehResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        Body = new AyandehVerifyTransactionRequest
                                                        {
                                                           Username = userName,
                                                           Password = password,
                                                           TraceNumber = request.ProviderTrackerId
                                                        },
                                                        BaseAddress = "https://mpg.ba24.ir/",
                                                        Uri = "mpg/api/ipgPurchaseVerify",
                                                        Provider = Enums.ProviderType.Ayandeh,
                                                        Service = Enums.ServiceType.AyandehVerify,
                                                    }, request, VerifyErrorHandler);

        var status = response.Result switch
        {
            "998" or "999" or "4034" => Enums.IPGTransactionStatus.Verifying,
            "0" => Enums.IPGTransactionStatus.VerificationSucceeded,
            _ => Enums.IPGTransactionStatus.VerificationFailed,
        };
        return new VerifyTransactionResponse
        {
            Status = status,            
            RRN = response.Rrn,
        };
    }

    public Task<SettleTransactionResponse> Settle(SettleTransactionRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{callbackPage.TrimEnd()}?track_id={trackerId}",
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
      where TResponse : AyandehTokenResponse
      where TError : AyandehResponseBase
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
      where TResponse : AyandehVerifyTransactionResponse
      where TError : AyandehResponseBase
      where TBaseRequest : VerifyTransactionRequest
    {
        return statusCode switch
        {
            504 => new VerifyTransactionResponse { Status = Enums.IPGTransactionStatus.Verifying } as TResponse,
            _ => verifyFailCounter < serviceCallMaxTryCounter ? await Retry() : new VerifyTransactionResponse {  Status = Enums.IPGTransactionStatus.VerificationFailed } as TResponse,
        };

        async Task<TResponse> Retry()
        {
            verifyFailCounter++;
            return await Verify(baseRequest) as TResponse;
        }
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(AyandehResponseBase? error)
       where TResponse : ResponseBase
       where TError : AyandehResponseBase
       where TBaseRequest : class

    {
        if (error is null || error.Result is null)
        {
            throw new Exception(GlobalResource.ProviderUnexpectedError);
        }
        if (!string.IsNullOrEmpty(error.Description))
        {
            throw new Exception($"ServiceProviderError: {error.Description}");
        }

        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }
}