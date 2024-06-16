using System;
using System.Net;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using ConfirmPecServiceReference;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Helper;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.DbContexts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PecServiceReference1;
namespace CPG.Infrastructure.Providers.Ipg;

public class PecProvider(
    ReadDbContext context,IApplicationSettingsRepository applicationSettingsRepository,ILogService logService,ILogger<PecProvider> logger) : IIpgProvider
{
    private readonly IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly ILogService _logService = logService;
    private readonly ILogger<PecProvider> _logger = logger;
    private readonly ReadDbContext context = context;

    public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        var configViewModel = await _applicationSettingRepositoy.GetAllApplicationSettings();
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);
        string callBack = CreateCallbackUrl((short)request.IpgRedirectionMethodType, request.SiteAddress,
            trackerId.ToString(), configViewModel);

        try
        {
            using (var SaleSvc = new SaleServiceSoapClient(SaleServiceSoapClient.EndpointConfiguration.SaleServiceSoap))
            {
                var clientSaleRequestData = new ClientSaleRequestData()
                {
                    AdditionalData =request.NationalCodeMatchingRequied ?
                    CreateAdditionalData(request.NationalCode, request.ShaparakKey, request.ShaparakIv, request.ThirdPartyCode): string.Empty,
                    Amount = (long)request.PaymentRequestAmount,
                    CallBackUrl = callBack,
                    LoginAccount = GetDataFromJsonProvider(request.ProviderData),
                    OrderId = long.Parse(trackerId),
                    Originator =string.IsNullOrEmpty( request.MobileNumber)?null:request.MobileNumber,
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

                CreateLog(clientSaleRequestData, response, nameof(SaleServiceSoapClient), response.Body.SalePaymentRequestResult.Status,
                          response.Body.SalePaymentRequestResult.Message, Enums.ServiceType.PecToken);
                return paymentResponse;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(SaleServiceSoapClient));
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
            using (var confirmSvc = new ConfirmServiceSoapClient(ConfirmServiceSoapClient.EndpointConfiguration.ConfirmServiceSoap))
            {
                var request = new ClientConfirmRequestData
                {
                    LoginAccount = GetDataFromJsonProvider(transactionResultRequest.ProviderData),
                    Token = long.Parse(transactionResultRequest.Token)
                };
                var confirm = await confirmSvc.ConfirmPaymentAsync(request);
                CreateLog(request, confirm, nameof(ConfirmServiceSoapClient), confirm.Body.ConfirmPaymentResult.Status,string.Empty,Enums.ServiceType.PecVerify);

                var CardNumberMasked = confirm.Body.ConfirmPaymentResult.CardNumberMasked;
                var RRN = confirm.Body.ConfirmPaymentResult.RRN.ToString();
                var Token = confirm.Body.ConfirmPaymentResult.Token;
                var Status = confirm.Body.ConfirmPaymentResult.Status;
                return ResponseModel(Status, RRN);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, nameof(ConfirmServiceSoapClient));
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
    private void CreateLog<T1, T2>(T1 request, T2 response, string serviceName,short status,string message, Enums.ServiceType serviceType)
    {
        _logService.ServiceName = serviceName;
        _logService.ServiceType = serviceType;
        _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;

        _logService.AddServiceCallLog(JsonConvert.SerializeObject(request),
            JsonConvert.SerializeObject(response),status,message);
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
    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId,
        ApplicationConfigViewModel applicationConfig) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{applicationConfig.Callback_Page}?pcu={applicationConfig.CPG_BackEnd.TrimEnd()}/IPGResult/p/b/{trackerId}",
        2 => $"{siteAddress}/p/b/{trackerId}?pcu={applicationConfig.CPG_BackEnd.TrimEnd()}/IPGResult",
        _ => string.Empty,
    };

}
