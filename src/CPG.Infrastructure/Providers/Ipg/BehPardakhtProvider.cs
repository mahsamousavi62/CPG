using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
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
using CPG.Domain.SharedKernel.Logging;
using BehPardakhtServiceReference;

namespace CPG.Infrastructure.Providers.Ipg;

public class BehPardakhtProvider(
    ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository, ILogService logService) : IIpgProvider
{
    private readonly IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly ILogService _logService = logService;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte settleFailCounter = 0;
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

        // Serialize full request object before try block for error logging
        bpPayRequest payRequest = null;
        string requestBodyJson = null;

        try
        {
            using (var client = new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort))
            {
                payRequest = new bpPayRequest
                {
                    Body = new bpPayRequestBody
                    {
                        terminalId = terminalId,
                        userName = userName,
                        userPassword = password,
                        orderId = long.Parse(trackerId),
                        amount = (long)request.PaymentRequestAmount,
                        localDate = DateTime.Now.ToString("yyyyMMdd"),
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

                // Serialize and mask request body for error logging
                requestBodyJson = JsonConvert.SerializeObject(payRequest);
                if (!string.IsNullOrEmpty(requestBodyJson))
                {
                    requestBodyJson = System.Text.RegularExpressions.Regex.Replace(requestBodyJson, Constants.Pattern, Constants.Replaceformat);
                }

                var response = await client.bpPayRequestAsync(payRequest.Body.terminalId, payRequest.Body.userName, payRequest.Body.userPassword,
                    payRequest.Body.orderId, payRequest.Body.amount, payRequest.Body.localDate, payRequest.Body.localTime, payRequest.Body.additionalData,
                    payRequest.Body.callBackUrl, payRequest.Body.payerId, payRequest.Body.mobileNo, payRequest.Body.encPan, payRequest.Body.panHiddenMode,
                    payRequest.Body.cartItem, payRequest.Body.enc);


                var responseData = response.Body.@return.Split(',');

                short status = short.TryParse(responseData[0], out short value) ? value : (short)1;
                string token = string.Empty;
                if (responseData.Length > 1)
                {
                    token = responseData[1];
                }
                var paymentResponse = new PaymentTokenResponse
                {
                    Token = token,
                    StatusCode = status == 0 ? (short)HttpStatusCode.OK : status,
                    IpgBaseUrl = request.IpgBaseUrl,
                    TrackerId = trackerId.ToString(),
                };

                CreateLog(payRequest, response, nameof(PaymentGatewayClient.bpPayRequestAsync), status, response.Body.@return, Enums.ServiceType.BehPardakhtToken);
                return paymentResponse;
            }
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: "BehPardakhtToken",
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpPayRequestAsync),
                requestBody: requestBodyJson,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtToken,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);

            if (tokenFailCounter < serviceCallMaxTryCounter)
            {
                await Retry();
            }
            throw new Exception(exc.Message, exc);

            async Task<PaymentTokenResponse> Retry()
            {
                tokenFailCounter++;
                return await GetPaymentTokenAsync(request);
            }
        }
    }

    public async Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        // Serialize full request object before try block for error logging
        bpVerifyRequest verifyRequest = null;
        string requestBodyJson = null;

        try
        {
            GetDataFromJsonProvider(request.ProviderData);
            using (var client = new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort))
            {
                verifyRequest = new bpVerifyRequest
                {
                    Body = new bpVerifyRequestBody
                    {
                        terminalId = terminalId,
                        userName = userName,
                        userPassword = password,
                        orderId = long.Parse(request.TrackId),
                        saleOrderId = long.Parse(request.TrackId),
                        saleReferenceId = long.Parse(request.ReferenceNumber),
                    }
                };

                // Serialize and mask request body for error logging
                requestBodyJson = JsonConvert.SerializeObject(verifyRequest);
                if (!string.IsNullOrEmpty(requestBodyJson))
                {
                    requestBodyJson = System.Text.RegularExpressions.Regex.Replace(requestBodyJson, Constants.Pattern, Constants.Replaceformat);
                }

                var response = await client.bpVerifyRequestAsync(verifyRequest.Body.terminalId, verifyRequest.Body.userName, verifyRequest.Body.userPassword,
                    verifyRequest.Body.orderId, verifyRequest.Body.saleOrderId, verifyRequest.Body.saleReferenceId);

                short status = short.TryParse(response.Body.@return, out short value) ? value : (short)1;

                CreateLog(request, response, nameof(PaymentGatewayClient.bpVerifyRequestAsync), status, string.Empty, Enums.ServiceType.BehPardakhtVerify);

                return new VerifyTransactionResponse
                {
                    Status = status switch
                    {
                        23 or 34 => Enums.IPGTransactionStatus.Verifying,
                        0 or 43 => Enums.IPGTransactionStatus.VerificationSucceeded,
                        _ => Enums.IPGTransactionStatus.VerificationFailed,
                    }
                };
            }
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: "BehPardakhtVerify",
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpVerifyRequestAsync),
                requestBody: requestBodyJson,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtVerify,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);

            if (verifyFailCounter < serviceCallMaxTryCounter)
            {
                await Retry();
            }
            throw new Exception(exc.Message, exc);

            async Task<VerifyTransactionResponse> Retry()
            {
                verifyFailCounter++;
                return await Verify(request);
            }
        }
    }

    public async Task<SettleTransactionResponse> Settle(SettleTransactionRequest request)
    {
        // Serialize full request object before try block for error logging
        bpSettleRequest settleRequest = null;
        string requestBodyJson = null;

        try
        {
            GetDataFromJsonProvider(request.ProviderData);
            using (var client = new PaymentGatewayClient(PaymentGatewayClient.EndpointConfiguration.PaymentGatewayImplPort))
            {
                settleRequest = new bpSettleRequest
                {
                    Body = new bpSettleRequestBody
                    {
                        terminalId = terminalId,
                        userName = userName,
                        userPassword = password,
                        orderId = long.Parse(request.TrackId),
                        saleOrderId = long.Parse(request.TrackId),
                        saleReferenceId = long.Parse(request.ReferenceNumber),
                    }
                };

                // Serialize and mask request body for error logging
                requestBodyJson = JsonConvert.SerializeObject(settleRequest);
                if (!string.IsNullOrEmpty(requestBodyJson))
                {
                    requestBodyJson = System.Text.RegularExpressions.Regex.Replace(requestBodyJson, Constants.Pattern, Constants.Replaceformat);
                }

                var response = await client.bpSettleRequestAsync(settleRequest.Body.terminalId, settleRequest.Body.userName, settleRequest.Body.userPassword,
                    settleRequest.Body.orderId, settleRequest.Body.saleOrderId, settleRequest.Body.saleReferenceId);

                short status = short.TryParse(response.Body.@return, out short value) ? value : (short)1;

                CreateLog(request, response, nameof(PaymentGatewayClient.bpSettleRequestAsync), status, string.Empty, Enums.ServiceType.BehPardakhtVerify);

                return new SettleTransactionResponse
                {
                    Status = status switch
                    {
                        23 or 34 => Enums.IPGTransactionStatus.WaitingForSettlementRequest,
                        0 or 45 => Enums.IPGTransactionStatus.SettlementSucceeded,
                        _ => Enums.IPGTransactionStatus.SettlementFailed,
                    }
                };
            }
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: "BehPardakhtSettle",
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpSettleRequestAsync),
                requestBody: requestBodyJson,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtSettle,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);

            if (settleFailCounter < serviceCallMaxTryCounter)
            {
                await Retry();
            }
            throw new Exception(exc.Message, exc);

            async Task<SettleTransactionResponse> Retry()
            {
                settleFailCounter++;
                return await Settle(request);
            }
        }
    }

    private void CreateLog<T1, T2>(T1 request, T2 response, string serviceName, short status, string message, Enums.ServiceType serviceType)
    {
        _logService.ServiceName = serviceName;
        _logService.ServiceType = serviceType;
        _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.Pec;

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
        return token;
    }
    private static string CreateCallbackUrl(short ipgRedirectionType, string siteAddress, string trackerId, string callbackPage) => ipgRedirectionType switch
    {
        1 => $"{siteAddress}/{callbackPage}?track_id={trackerId}",
        2 => $"{siteAddress}/p/b/{trackerId}?pcu={callbackPage.TrimEnd()}/IPGResult",
        _ => string.Empty,
    };
}