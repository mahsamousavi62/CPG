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
using CPG.Domain.SharedKernel.Communication.Ipg.Vandar;
using CPG.Domain.SharedKernel.Helper;
using CPG.Infrastructure.Persistence.DbContexts;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.Ipg;

internal class VandarProvider(IHttpProvider httpProvider, ReadDbContext context, IApplicationSettingsRepository applicationSettingsRepository) : IIpgProvider
{
    public IApplicationSettingsRepository _applicationSettingRepositoy = applicationSettingsRepository;
    private readonly IHttpProvider httpProvider = httpProvider;
    private readonly ReadDbContext context = context;
    private readonly byte serviceCallMaxTryCounter = 5;
    private byte tokenFailCounter = 0;
    private byte verifyFailCounter = 0;
    private byte transactionResultFailCounter = 0;
    private string refreshToken;

    private void GetDataFromJsonProvider(string providerData)
    {
        dynamic jsonObjectProviderData;
        try
        {
            jsonObjectProviderData = JObject.Parse(providerData);
            refreshToken = jsonObjectProviderData["Refresh_Token"] is not null ? jsonObjectProviderData["Refresh_Token"] : throw new Exception("Invalid Refresh_Token");
        }
        catch
        {
            throw new ParseCompanyIpgProviderDataException(providerData);
        }
    }

    public async Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest request)
    {
        GetDataFromJsonProvider(request.ProviderData);
        var trackerId = RandomGenerator.GenerateRandomDigitNumber(16);

        var response = await httpProvider.PostAsync<PaymentTokenRequest, VandarTokenResponse,
                                                    VandarResponseBase, dynamic>(new HttpProviderRequest<dynamic>
                                                    {
                                                        BaseAddress = "https://api.vandar.io/",
                                                        Uri = "v3/refreshtoken",
                                                        Body = new VandarTokenRequest
                                                        {
                                                            RefreshToken = refreshToken,
                                                        },
                                                        Provider = Enums.ProviderType.Sep,
                                                        Service = Enums.ServiceType.SepToken,
                                                    }, request, PaymentTokenErrorHandler);

        return new PaymentTokenResponse
        {
            StatusCode = response.StatusCode,
            Token = response.AccessToken,
            IpgBaseUrl = request.IpgBaseUrl,
            RefreshToken = response.RefreshToken,
            ExpiresIn = response.ExpiresIn,  
            TrackerId = trackerId,
        };
    }

    private async Task<TResponse?> PaymentTokenErrorHandler<TBaseRequest, TResponse, TError>(TBaseRequest? baseRequest, TResponse? response, TError? error, short statusCode)
        where TResponse : VandarTokenResponse
        where TError : VandarResponseBase
        where TBaseRequest : PaymentTokenRequest
    {
        if (tokenFailCounter < serviceCallMaxTryCounter)
        {
            tokenFailCounter++;
            return await GetPaymentTokenAsync(baseRequest) as TResponse;
        }
        return await Task.FromResult(BaseErrorHandler<TResponse, TError, TBaseRequest>(error));
    }

    private TResponse BaseErrorHandler<TResponse, TError, TBaseRequest>(VandarResponseBase? error)
        where TResponse : ResponseBase
        where TError : VandarResponseBase
        where TBaseRequest : class
    {
        if (error is null)
        {
            throw new Exception(GlobalResource.ProviderUnexpectedError);
        }
        
        throw new Exception(GlobalResource.ProviderUnexpectedError);
    }

    public Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }

    public Task<VerifyTransactionResponse> Verify(VerifyTransactionRequest transactionResultRequest)
    {
        throw new NotImplementedException();
    }
}
