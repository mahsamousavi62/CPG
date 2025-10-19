using CCPG.Domain.SharedKernel.Communication.Ipg;
using ConfirmPecServiceReference;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Policies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PecServiceReference1;
using System;
using System.Net;
using System.Threading.Tasks;
namespace CPG.Infrastructure.Providers.Ipg;

public class PecProvider(
    ReadDbContext context,
    IApplicationSettingsRepository applicationSettingsRepository,
    ILogService logService,
    IPollyPolicyService pollyPolicyService) : IIpgProvider
{
    private readonly IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly ILogService _logService = logService;
    private readonly ReadDbContext context = context;
    private readonly IPollyPolicyService _pollyPolicyService = pollyPolicyService;

    public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        var configViewModel = await _applicationSettingRepositoy.GetAllApplicationSettings();

        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);

        string callBack = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress,
            trackerId.ToString(), configViewModel);

        ClientSaleRequestData clientSaleRequestData = null;
        var startTime = DateTime.Now;

        try
        {
            return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
            {
                using (var SaleSvc = new SaleServiceSoapClient(SaleServiceSoapClient.EndpointConfiguration.SaleServiceSoap))
                {
                    clientSaleRequestData = new ClientSaleRequestData()
                    {
                        AdditionalData = request.NationalCodeMatchingRequied ?
                       CreateAdditionalData(request.NationalCode,
                                             request.ShaparakKey,
                                             request.ShaparakIv,
                                             request.ThirdPartyCode,
                                             request.PaymentIdentifier) : JsonConvert.SerializeObject(new { Data = request.PaymentIdentifier }),
                        Amount = (long)request.PaymentRequestAmount,
                        CallBackUrl = callBack,
                        LoginAccount = GetDataFromJsonProvider(request.ProviderData),
                        OrderId = long.Parse(trackerId),
                        Originator = string.IsNullOrWhiteSpace(request.MobileNumber) ? null : request.MobileNumber.Trim(),
                    };

                    var response = await SaleSvc.SalePaymentRequestAsync(clientSaleRequestData);

                    int status = response.Body.SalePaymentRequestResult.Status;
                    long token = response.Body.SalePaymentRequestResult.Token;

                    PaymentTokenResponse paymentResponse = new PaymentTokenResponse
                    {
                        Token = token.ToString(),
                        StatusCode = status == 0 && token > 0 ? (short)HttpStatusCode.OK : (short)status,
                        Message = response.Body.SalePaymentRequestResult.Message,
                        IpgBaseUrl = request.IpgBaseUrl,
                        TrackerId = trackerId.ToString(),
                    };

                    _logService.ServiceName = nameof(SaleServiceSoapClient.SalePaymentRequestAsync);
                    _logService.ServiceType = Enums.ServiceType.PecToken;
                    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;
                    _logService.AddSoapCallLog(clientSaleRequestData, response, nameof(SaleServiceSoapClient.SalePaymentRequestAsync),
                        (short)status, response.Body.SalePaymentRequestResult.Message);

                    return paymentResponse;
                }
            }, "PecProvider.GetPaymentToken");
        }
        catch (Exception ex) when (ex is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            var durationMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _logService.ServiceName = nameof(SaleServiceSoapClient.SalePaymentRequestAsync);
            _logService.ServiceType = Enums.ServiceType.PecToken;
            _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;

            if (clientSaleRequestData != null)
            {
                _logService.AddSoapTimeoutLog(clientSaleRequestData, nameof(SaleServiceSoapClient.SalePaymentRequestAsync), ex, durationMs);
            }

            throw;
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(SaleServiceSoapClient.SalePaymentRequestAsync),
                providerName: "Pec",
                requestUri: nameof(SaleServiceSoapClient.SalePaymentRequestAsync),
                requestBody: clientSaleRequestData != null ? Newtonsoft.Json.JsonConvert.SerializeObject(clientSaleRequestData) : null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.PecToken,
                providerType: Enums.ProviderTypeInLog.Pec,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest transactionResultRequest)
    {
        ClientConfirmRequestData confirmRequest = null;
        var startTime = DateTime.Now;

        try
        {
            return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
            {
                using (var confirmSvc = new ConfirmServiceSoapClient(ConfirmServiceSoapClient.EndpointConfiguration.ConfirmServiceSoap))
                {
                    confirmRequest = new ClientConfirmRequestData
                    {
                        LoginAccount = GetDataFromJsonProvider(transactionResultRequest.ProviderData),
                        Token = long.Parse(transactionResultRequest.Token)
                    };

                    var confirm = await confirmSvc.ConfirmPaymentAsync(confirmRequest);

                    _logService.ServiceName = nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync);
                    _logService.ServiceType = Enums.ServiceType.PecVerify;
                    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;
                    _logService.AddSoapCallLog(confirmRequest, confirm, nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync),
                        confirm.Body.ConfirmPaymentResult.Status, string.Empty);

                    var CardNumberMasked = confirm.Body.ConfirmPaymentResult.CardNumberMasked;
                    var RRN = confirm.Body.ConfirmPaymentResult.RRN.ToString();
                    var Token = confirm.Body.ConfirmPaymentResult.Token;
                    var Status = confirm.Body.ConfirmPaymentResult.Status;
                    return ResponseModel(Status, RRN);
                }
            }, "PecProvider.Verify");
        }
        catch (Exception ex) when (ex is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            var durationMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _logService.ServiceName = nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync);
            _logService.ServiceType = Enums.ServiceType.PecVerify;
            _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;

            if (confirmRequest != null)
            {
                _logService.AddSoapTimeoutLog(confirmRequest, nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync), ex, durationMs);
            }

            throw;
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync),
                providerName: "Pec",
                requestUri: nameof(ConfirmServiceSoapClient.ConfirmPaymentAsync),
                requestBody: confirmRequest != null ? Newtonsoft.Json.JsonConvert.SerializeObject(confirmRequest) : null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.PecVerify,
                providerType: Enums.ProviderTypeInLog.Pec,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);
            throw;
        }
    }

    public Task<SettleTransactionResponse> Settle(SettleTransactionRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    private string GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            return jsonObjectProviderData["PIN_Code"] is not null ? jsonObjectProviderData["PIN_Code"] : throw new Exception("Invalid PIN_Code");
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }
    private static VerifyTransactionResponse ResponseModel(short status, string rrn)
    {
        return new VerifyTransactionResponse
        {
            Status = status switch
            {
                -32768 or -1610 or -1604 or -1598 or 6 or 504 => Enums.IPGTransactionStatus.Verifying,
                0 or 2 => Enums.IPGTransactionStatus.VerificationSucceeded,
                -1528 or -1531 or 1530 => Enums.IPGTransactionStatus.VerificationFailed,
                _ => throw new NotImplementedException(),
            },
            RRN = rrn
        };
    }

    private string CreateAdditionalData(string nationalCode, string key, string iv, int? thirdParty, string paymentIdenetifier)
    {
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 7);
        var original = $"0|{nationalCode}|{randomString}";
        var dkey = AesHelper.Base64Decode(key);
        var div = AesHelper.Base64Decode(iv);
        var token = AesHelper.EncryptAes(original, dkey ?? string.Empty, div ?? string.Empty);
        var json = JsonConvert.SerializeObject(new { NationalEncryptedId = token, ThirdPartyCode = thirdParty, Data = paymentIdenetifier });

        return json;
    }
    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId,
        ApplicationConfigViewModel applicationConfig) => ipgRedirectionType switch
        {
            1 => $"{siteAddress}/{applicationConfig.Callback_Page}?pcu={applicationConfig.CPG_BackEnd.TrimEnd()}/IPGResult/p/b/{trackerId}",
            2 => $"{siteAddress}/p/b/{trackerId}?pcu={applicationConfig.CPG_BackEnd.TrimEnd()}/IPGResult",
            _ => string.Empty,
        };

}
