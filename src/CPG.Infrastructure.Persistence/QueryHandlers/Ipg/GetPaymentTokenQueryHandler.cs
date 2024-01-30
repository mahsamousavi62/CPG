using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class GetPaymentTicketQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> companyIPGRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAuthenticationService authenticationService,
    ReadDbContext context) : IRequestHandler<GetPaymentTokenCommand, Result<PaymentTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> _companyIPGRepository = companyIPGRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<PaymentTokenResponseViewModel>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode));
            if (paymentRequest is null) { throw new PaymentRequestNotFoundByCodeException(); }
            if (!paymentRequest.Company.IsActive) { throw new PaymentTokenInactiveCompanyException(); }
            if (paymentRequest.UrlExpirationDateTime < DateTime.UtcNow) { throw new PaymentRequestCodeExpiredException(); }
            if (paymentRequest.IsUsed) { throw new PaymentRequestCodeIsUsedBeforeException(); }
            if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) { throw new PaymentRequestCodeInvalidStatusException(); }

            var companyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGIncludeProvider(request.PaymentToken.CompanyIPGId));
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.PaymentToken.CompanyIPGId); }
            if (!companyIpg.IsActive) { throw new PaymentTokenInactiveIPGException(); }
            if (!companyIpg.IPGType.IsActive) { throw new PaymentTokenInactiveIPGTypeException(); }
            if (!companyIpg.Provider.IsActive) { throw new PaymentTokenInactiveProviderException(); }

            if (paymentRequest.Company.NationalCodeMatchingRequied is true &&
                (string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Iv) || string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Key)))
            {
                throw new PaymentTokenNullKeyOrIvException();
            }

            long destinationDepositId;
            if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationDepositIban))
            {
                var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(paymentRequest.DestinationDepositIban));
                if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
                if (companyDeposit.CompanyId != paymentRequest.CompanyId) { throw new PaymentTokenDepositNotBelongsCompanyException(); }
                if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
                if (!companyDeposit.Bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
                destinationDepositId = companyDeposit.Id;
            }
            else
            {
                var tempcompanyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGByIpgDeposit(companyIpg.Id));

                if (tempcompanyIpg.IPGDeposits.SingleOrDefault() is null)
                    throw new Exception("CompanyDeposit not found!");
                destinationDepositId = tempcompanyIpg.IPGDeposits.SingleOrDefault().CompanyDepositId;
            }

            var ipg = _ipgFactory.GetInstance(companyIpg.Provider.ProviderType);
            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);

            string ipgBaseUrl;
            short IpgVerificationTimeLimit;
            try
            {
                dynamic jsonObjectProviderData = JObject.Parse(companyIpg.Provider.ProviderData);
                ipgBaseUrl = jsonObjectProviderData["IPG_Base_URL"] is not null
                   ? (string)jsonObjectProviderData["IPG_Base_URL"] : throw new Exception("Invalid IPG_Base_URL");

                IpgVerificationTimeLimit = jsonObjectProviderData["IPG_Verification_Time_Limit"] is not null
                               ? (short)jsonObjectProviderData["IPG_Verification_Time_Limit"] : throw new Exception("IPG_Verification_TimeLimit");

            }
            catch
            {
                throw new ParseCompanyIpgProviderDataException(companyIpg.Provider.ProviderData);
            }

            var result = await ipg.GetPaymentTokenAsync(
                new PaymentTokenRequest
                {
                    ProviderData = companyIpg.ProviderData,
                    PaymentRequestAmount = paymentRequest.Amount,
                    IpgRedirectionMethodType = (Enums.IpgRedirectionMethodType)paymentRequest.Company.IpgRedirectionMethodType,
                    SiteAddress = paymentRequest.Company.SiteAddress,
                    IpgBaseUrl = ipgBaseUrl,
                    NationalCode = paymentRequest.NationalCode,
                    MobileNumber = mobileNumber,
                    NationalCodeMatchingRequied = paymentRequest.Company.NationalCodeMatchingRequied,
                    ShaparakIv = paymentRequest.Company.ShaparakSetting.Iv,
                    ShaparakKey = paymentRequest.Company.ShaparakSetting.Key,
                    ThirdPartyCode = paymentRequest.Company.ShaparakSetting.ThirdPartyCode,
                });

            if (result.StatusCode == (short)HttpStatusCode.OK)
            {
                Transaction transaction = Transaction.Create(new CreateTransactionModel
                {
                    IpgVerificationTimeLimit = IpgVerificationTimeLimit,
                    CompanyIPG = companyIpg,
                    DestinationDepositId = destinationDepositId,
                    PaymentRequest = paymentRequest,
                    Token = result.Token,
                    TrackId = result.TrackerId = result.TrackerId,
                    TransactionMethodType = Enums.TransactionType.IPG
                });
                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();
                PaymentRequest.Update(paymentRequest);
                await _paymentRequestRepository.UpdateAsync(paymentRequest);
                await _paymentRequestRepository.SaveChangesAsync();

                return CreateResponseModel(paymentRequest, companyIpg, mobileNumber, result);
            }
            else
            {
                return Result<PaymentTokenResponseViewModel>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
            }
        }
        catch (DomainException exc)
        {
            return Result<PaymentTokenResponseViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<PaymentTokenResponseViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<PaymentTokenResponseViewModel>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }

    private static Result<PaymentTokenResponseViewModel> CreateResponseModel(PaymentRequest paymentRequest, Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG companyIpg, string mobileNumber, PaymentTokenResponse result)
    {
        var response = new PaymentTokenResponseViewModel()
        {
            Url = $"{paymentRequest.Company.SiteAddress}/redirectToBank",
            JsonBody = new ExpandoObject(),
            RedirectionMethodType = paymentRequest.Company.IpgRedirectionMethodType,
        };

        switch (companyIpg.Provider.ProviderType)
        {
            case Enums.ProviderType.Vandar:
                break;
            case Enums.ProviderType.AsanPardakht:
                {
                    response.JsonBody.jsonStr = new ExpandoObject();
                    response.JsonBody.jsonStr.url = result.IpgBaseUrl;
                    response.JsonBody.jsonStr.method = "POST";
                    response.JsonBody.jsonStr.@params = new ExpandoObject();
                    response.JsonBody.jsonStr.@params.RefID = result.Token;

                    if (!string.IsNullOrEmpty(mobileNumber))
                        response.JsonBody.jsonStr.@params.Mobileap = mobileNumber;
                    
                    break;
                }
            case Enums.ProviderType.Sep:
                {
                    response.JsonBody.jsonStr = new ExpandoObject();
                    response.JsonBody.jsonStr.url = result.IpgBaseUrl;
                    response.JsonBody.jsonStr.method = "POST";
                    response.JsonBody.jsonStr.@params = new ExpandoObject();
                    response.JsonBody.jsonStr.@params.Token = result.Token;
                    response.JsonBody.jsonStr.@params.GetMethod = false;
                    break;
                }
            case Enums.ProviderType.Pec:
                {
                    response.JsonBody.jsonStr = new ExpandoObject();
                    response.JsonBody.jsonStr.url = result.IpgBaseUrl;
                    response.JsonBody.jsonStr.method = "POST";
                    response.JsonBody.jsonStr.@params = new ExpandoObject();
                    response.JsonBody.jsonStr.@params.Token = result.Token;
                    break;
                }
            default:
                break;
        }
        return Result<PaymentTokenResponseViewModel>.SuccessResult(response);
    }
}



