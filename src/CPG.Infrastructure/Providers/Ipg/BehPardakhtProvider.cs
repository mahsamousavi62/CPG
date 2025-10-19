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
using CPG.Infrastructure.Policies;

namespace CPG.Infrastructure.Providers.Ipg;

public class BehPardakhtProvider(
    ReadDbContext context,
    IApplicationSettingsRepository applicationSettingsRepository,
    ILogService logService,
    IPollyPolicyService pollyPolicyService) : IIpgProvider
{
    private readonly IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly ILogService _logService = logService;
    private readonly ReadDbContext context = context;
    private readonly IPollyPolicyService _pollyPolicyService = pollyPolicyService;
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

        bpPayRequest payRequest = null;
        var startTime = DateTime.Now;

        try
        {
            return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
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

                    _logService.ServiceName = nameof(PaymentGatewayClient.bpPayRequestAsync);
                    _logService.ServiceType = Enums.ServiceType.BehPardakhtToken;
                    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;
                    _logService.AddSoapCallLog(payRequest, response, nameof(PaymentGatewayClient.bpPayRequestAsync), status, response.Body.@return);

                    return paymentResponse;
                }
            }, "BehPardakhtProvider.GetPaymentToken");
        }
        catch (Exception ex) when (ex is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            var durationMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _logService.ServiceName = nameof(PaymentGatewayClient.bpPayRequestAsync);
            _logService.ServiceType = Enums.ServiceType.BehPardakhtToken;
            _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;

            if (payRequest != null)
            {
                _logService.AddSoapTimeoutLog(payRequest, nameof(PaymentGatewayClient.bpPayRequestAsync), ex, durationMs);
            }

            throw;
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(PaymentGatewayClient.bpPayRequestAsync),
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpPayRequestAsync),
                requestBody: payRequest != null ? JsonConvert.SerializeObject(payRequest) : null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtToken,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
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

    public async Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        bpVerifyRequest verifyRequest = null;
        var startTime = DateTime.Now;

        try
        {
            return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
            {
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

                    var response = await client.bpVerifyRequestAsync(verifyRequest.Body.terminalId, verifyRequest.Body.userName, verifyRequest.Body.userPassword,
                        verifyRequest.Body.orderId, verifyRequest.Body.saleOrderId, verifyRequest.Body.saleReferenceId);

                    short status = short.TryParse(response.Body.@return, out short value) ? value : (short)1;

                    _logService.ServiceName = nameof(PaymentGatewayClient.bpVerifyRequestAsync);
                    _logService.ServiceType = Enums.ServiceType.BehPardakhtVerify;
                    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;
                    _logService.AddSoapCallLog(verifyRequest, response, nameof(PaymentGatewayClient.bpVerifyRequestAsync), status, string.Empty);

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
            }, "BehPardakhtProvider.Verify");
        }
        catch (Exception ex) when (ex is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            var durationMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _logService.ServiceName = nameof(PaymentGatewayClient.bpVerifyRequestAsync);
            _logService.ServiceType = Enums.ServiceType.BehPardakhtVerify;
            _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;

            if (verifyRequest != null)
            {
                _logService.AddSoapTimeoutLog(verifyRequest, nameof(PaymentGatewayClient.bpVerifyRequestAsync), ex, durationMs);
            }

            throw;
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(PaymentGatewayClient.bpVerifyRequestAsync),
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpVerifyRequestAsync),
                requestBody: verifyRequest != null ? JsonConvert.SerializeObject(verifyRequest) : null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtVerify,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);
            throw;
        }
    }

    public async Task<SettleTransactionResponse> Settle(SettleTransactionRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        bpSettleRequest settleRequest = null;
        var startTime = DateTime.Now;

        try
        {
            return await _pollyPolicyService.ExecuteWithPolicyAsync(async () =>
            {
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

                    var response = await client.bpSettleRequestAsync(settleRequest.Body.terminalId, settleRequest.Body.userName, settleRequest.Body.userPassword,
                        settleRequest.Body.orderId, settleRequest.Body.saleOrderId, settleRequest.Body.saleReferenceId);

                    short status = short.TryParse(response.Body.@return, out short value) ? value : (short)1;

                    _logService.ServiceName = nameof(PaymentGatewayClient.bpSettleRequestAsync);
                    _logService.ServiceType = Enums.ServiceType.BehPardakhtSettle;
                    _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;
                    _logService.AddSoapCallLog(settleRequest, response, nameof(PaymentGatewayClient.bpSettleRequestAsync), status, string.Empty);

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
            }, "BehPardakhtProvider.Settle");
        }
        catch (Exception ex) when (ex is TimeoutException || ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            var durationMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _logService.ServiceName = nameof(PaymentGatewayClient.bpSettleRequestAsync);
            _logService.ServiceType = Enums.ServiceType.BehPardakhtSettle;
            _logService.ProviderTypeInLog = Enums.ProviderTypeInLog.BehPardakht;

            if (settleRequest != null)
            {
                _logService.AddSoapTimeoutLog(settleRequest, nameof(PaymentGatewayClient.bpSettleRequestAsync), ex, durationMs);
            }

            throw;
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(PaymentGatewayClient.bpSettleRequestAsync),
                providerName: "BehPardakht",
                requestUri: nameof(PaymentGatewayClient.bpSettleRequestAsync),
                requestBody: settleRequest != null ? JsonConvert.SerializeObject(settleRequest) : null,
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.BehPardakhtSettle,
                providerType: Enums.ProviderTypeInLog.BehPardakht,
                auditType: Enums.AuditType.Provider,
                userId: 1
            );
            _logService.LogError(callLog);
            throw;
        }
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