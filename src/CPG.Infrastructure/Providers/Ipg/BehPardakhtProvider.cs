using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using ConfirmPecServiceReference;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.Extensions.Logging;
using BehPardakhtServiceReference;

namespace CPG.Infrastructure.Providers.Ipg;

public class BehPardakhtProvider(
    ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository, ILogService logService, ILogger<PecProvider> logger) : IIpgProvider
{
    private readonly IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly ILogService _logService = logService;
    private readonly ILogger<PecProvider> _logger = logger;
    private readonly ReadDbContext context = context;
    private string userName;
    private string password;
    private int terminalId;

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            terminalId = jsonObjectProviderData["Terminal_ID"] is not null ? (int)jsonObjectProviderData["Terminal_ID"] : throw new Exception("Invalid TerminalId");
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
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        string callBackUrl = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress, trackerId.ToString(), configViewModel.CPG_BackEnd);

        try
        {
            using (var clientPort = new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort))
            {
                var payRequest = new bpPayRequest
                {
                    Body = new bpPayRequestBody
                    {
                        terminalId = terminalId,
                        userName = userName,
                        userPassword = password,
                        orderId = long.Parse(trackerId),
                        amount = (long)request.PaymentRequestAmount,
                        localDate = DateTime.Now.ToString("YYYYMMDD"),
                        localTime = DateTime.Now.ToString("HHmmss"),
                        callBackUrl = callBackUrl,
                        payerId = "0",
                        mobileNo = !string.IsNullOrEmpty(request.MobileNumber) ? $"98{request.MobileNumber.Remove(0, 1)}" : null,
                        encPan = null,
                        panHiddenMode = null,                        
                        enc = request.NationalCodeMatchingRequied ? CreateAdditionalData(request.NationalCode, request.ShaparakKey, request.ShaparakIv, request.ThirdPartyCode) : string.Empty,
                        cartItem = null,
                        additionalData = null,
                    }
                };
                var response = await clientPort.bpPayRequestAsync(payRequest.Body.terminalId, payRequest.Body.userName, payRequest.Body.userPassword,
                    payRequest.Body.orderId, payRequest.Body.amount, payRequest.Body.localDate, payRequest.Body.localTime, payRequest.Body.additionalData,
                    payRequest.Body.callBackUrl, payRequest.Body.payerId, payRequest.Body.mobileNo, payRequest.Body.encPan, payRequest.Body.panHiddenMode,
                    payRequest.Body.cartItem, payRequest.Body.enc);

                var responseData = response.Body.@return.Split(',');
                short status = short.TryParse(responseData[0], out short value) ? value : (short)1;
                string token = responseData[1];

                PaymentTokenResponse paymentResponse = new PaymentTokenResponse
                {
                    Token = token.ToString(),
                    StatusCode = status == 0 ? (short)HttpStatusCode.OK : status,                    
                    IpgBaseUrl = request.IpgBaseUrl,
                    TrackerId = trackerId.ToString(),
                };

                CreateLog(payRequest, response, nameof(PaymentGatewayClient), status, response.Body.@return, Enums.ServiceType.BehPardakhtToken);
                return paymentResponse;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(PaymentGatewayClient));
            throw;
        }
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest transactionResultRequest)
    {
        try
        {
            GetDataFromJsonProvider(transactionResultRequest.ProviderData);
            using (var confirmSvc = new ConfirmServiceSoapClient(ConfirmServiceSoapClient.EndpointConfiguration.ConfirmServiceSoap))
            {
                var request = new ClientConfirmRequestData
                {   
                    Token = long.Parse(transactionResultRequest.Token)
                };
                var confirm = await confirmSvc.ConfirmPaymentAsync(request);
                CreateLog(request, confirm, nameof(ConfirmServiceSoapClient), confirm.Body.ConfirmPaymentResult.Status, string.Empty, Enums.ServiceType.PecVerify);

                var CardNumberMasked = confirm.Body.ConfirmPaymentResult.CardNumberMasked;
                var RRN = confirm.Body.ConfirmPaymentResult.RRN;
                var Token = confirm.Body.ConfirmPaymentResult.Token;
                var Status = confirm.Body.ConfirmPaymentResult.Status;
                return ResponseModel(Status);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(ConfirmServiceSoapClient));
            throw;
        }
    }

    private static VerifyTransactionResponse ResponseModel(short status)
    {
        return new VerifyTransactionResponse
        {
            Status = status switch
            {
                -32768 or -1610 or -1604 or -1598 or 6 or 504 => Enums.IPGTransactionStatus.Verifying,
                0 or 2 => Enums.IPGTransactionStatus.VerificationSucceeded,
                -1528 or -1531 or 1530 => Enums.IPGTransactionStatus.VerificationFailed,
                _ => throw new NotImplementedException(),
            }
        };
    }
    private void CreateLog<T1, T2>(T1 request, T2 response, string serviceName, short status, string message, Enums.ServiceType serviceType)
    {
        _logService.ServiceName = serviceName;
        _logService.ServiceType = serviceType;
        _logService.ProviderType = Enums.ProviderType.Pec;

        _logService.AddServiceCallLog(JsonConvert.SerializeObject(request),
            JsonConvert.SerializeObject(response), status, message);
    }
    private string CreateAdditionalData(string nationalCode, string key, string iv, int? thirdParty)
    {
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 7);
        var original = $"0|{nationalCode}|{randomString}";
        var dkey = AesHelper.Base64Decode(key);
        var div = AesHelper.Base64Decode(iv);
        var token = AesHelper.EncryptAes(original, dkey ?? string.Empty, div ?? string.Empty);
        var json = JsonConvert.SerializeObject(new { NationalEncryptedId = token, ThirdPartyCode = thirdParty, Data = string.Empty });
        return json;
    }
    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{callbackPage}?track_id={trackerId}",
        2 => $"{siteAddress}/p/b/{trackerId}?pcu={callbackPage.TrimEnd()}/IPGResult",
        _ => string.Empty,
    };

}