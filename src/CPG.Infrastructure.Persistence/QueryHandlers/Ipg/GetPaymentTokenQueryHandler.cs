using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> companyIPGRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    IDirectDebitFactory directDebitFactory,
    IAuthenticationService authenticationService,
    ReadDbContext context,
    IHttpContextAccessor httpContext,
    IAggregateRepository<User> userRepository
    ) : IRequestHandler<GetPaymentTokenCommand, Result<PaymentTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> _companyIPGRepository = companyIPGRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _grantRepository = grantRepository;
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IAggregateRepository<User> _userRepository = userRepository;

    public async Task<Result<PaymentTokenResponseViewModel>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode), cancellationToken);
            if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();
            if (!paymentRequest.Company.IsActive) throw new PaymentTokenInactiveCompanyException();
            if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
            if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
            if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

            if (paymentRequest.Company.NationalCodeMatchingRequied is true &&
                (string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Iv) || string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Key)))
            {
                throw new PaymentTokenNullKeyOrIvException();
            }

            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);

            var companyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGIncludeProvider((long)request.PaymentToken.CompanyIPGId), cancellationToken);
            if (companyIpg is null) throw new CompanyIPGNotFoundException((long)request.PaymentToken.CompanyIPGId);
            if (!companyIpg.IsActive) throw new PaymentTokenInactiveIPGException();
            if (!companyIpg.IPGType.IsActive) throw new PaymentTokenInactiveIPGTypeException();
            if (!companyIpg.Provider.IsActive) throw new PaymentTokenInactiveProviderException();
            if (!companyIpg.Provider.PaymentMethods.Any(t => t.MethodType == Enums.PaymentMethodType.InternetPaymentGateway)) throw new PaymentRequestProviderHasNoIPGMethodException();

            long destinationDepositId;
            Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit companyDeposit;
            if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationDepositIban))
            {
                companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(paymentRequest.DestinationDepositIban), cancellationToken);
                if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
                if (companyDeposit.CompanyId != paymentRequest.CompanyId) { throw new PaymentTokenDepositNotBelongsCompanyException(); }
            }
            else
            {
                var tempcompanyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGByIpgDeposit(companyIpg.Id), cancellationToken);
                companyDeposit = tempcompanyIpg.IPGDeposits.SingleOrDefault().CompanyDeposit;
                if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
            }

            if (!companyDeposit.IsActive) throw new PaymentTokenInactiveDepositException();
            if (!companyDeposit.Bank.IsActive) throw new PaymentTokenInactiveBankException();
            if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.InternetPaymentGateway) is false) throw new PaymentRequestCompanyHasNoIPGMethodException();
            destinationDepositId = companyDeposit.Id;

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

            var ipg = _ipgFactory.GetInstance(companyIpg.Provider.ProviderType);
            string mobileNumber = string.Empty;
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);
            User user = null;
            var nationalCode = paymentRequest.NationalCode;
            List<Claim> claims = new();
            if (string.IsNullOrEmpty(sub))
            {
                user = await _userRepository.GetBySpecAsync(new UserByNationalCodeSpec(nationalCode));
                if (user is not null && !string.IsNullOrEmpty(user.PhoneNumber) && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    mobileNumber = user.PhoneNumber;
                    claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber, ClaimValueTypes.String));
                }
            }
            else
            {
                mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
            }

            var result = await ipg.GetPaymentTokenAsync(
                new PaymentTokenRequest
                {
                    ProviderData = companyIpg.ProviderData,
                    PaymentRequestAmount = paymentRequest.Amount,
                    IpgRedirectionMethodType = paymentRequest.Company.IpgRedirectionMethodType,
                    SiteAddress = paymentRequest.Company.SiteAddress,
                    IpgBaseUrl = ipgBaseUrl,
                    NationalCode = paymentRequest.NationalCode,
                    MobileNumber = mobileNumber,
                    NationalCodeMatchingRequied = paymentRequest.Company.NationalCodeMatchingRequied,
                    ShaparakIv = paymentRequest.Company.NationalCodeMatchingRequied ? paymentRequest.Company.ShaparakSetting.Iv : string.Empty,
                    ShaparakKey = paymentRequest.Company.NationalCodeMatchingRequied ? paymentRequest.Company.ShaparakSetting.Key : string.Empty,
                    ThirdPartyCode = paymentRequest.Company.NationalCodeMatchingRequied ? paymentRequest.Company.ShaparakSetting.ThirdPartyCode : null,
                    PaymentIdentifier = paymentRequest.PaymentIdentifier,
                });

            if (result.StatusCode == (short)HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(sub))
                {
                    if (user is null)
                    {
                        user = User.Create(nationalCode);
                        await _userRepository.AddAsync(user);
                    }

                    claims.AddRange(new List<Claim> { new Claim("UserId", user?.Id.ToString()) ,
                                                      new Claim("NationalCode", value: user?.NationalCode)});

                    var appIdentity = new ClaimsIdentity(claims);
                    _httpContext.HttpContext.User.AddIdentity(appIdentity);
                }

                Transaction transaction = Transaction.Create(new CreateTransactionModel
                {
                    DestinationDepositId = destinationDepositId,
                    PaymentRequest = paymentRequest,
                    TransactionMethodType = Enums.TransactionType.IPG,
                    Status = Enums.TransactionStatus.InPrgress,
                    IPGTransactionModel = new CreateIPGTransactionModel
                    {
                        Token = result.Token,
                        TrackId = result.TrackerId = result.TrackerId,
                        IpgVerificationTimeLimit = IpgVerificationTimeLimit,
                        CompanyIPG = companyIpg,
                    }
                });
                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();
                paymentRequest.IsUsed = true;
                paymentRequest.Status = Enums.PaymentStatus.InProgress;
                PaymentRequest.Update(paymentRequest);
                await _paymentRequestRepository.UpdateAsync(paymentRequest);
                await _paymentRequestRepository.SaveChangesAsync();

            }
            else
            {
                return Result<PaymentTokenResponseViewModel>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
            }
            return CreateResponseModel(paymentRequest, companyIpg, mobileNumber, result);
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
        var url = string.Empty;

        var response = new PaymentTokenResponseViewModel()
        {
            Url = paymentRequest.Company.IpgRedirectionMethodType == Enums.IpgRedirectionMethodType.RayanReferencePage ?
            $"{paymentRequest.Company.SiteAddress}/redirectToBank" : $"{paymentRequest.Company.SiteAddress}/redirect-to-bank",
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
                    response.JsonBody.jsonStr.method = "GET";
                    response.JsonBody.jsonStr.@body = new ExpandoObject();
                    response.JsonBody.jsonStr.@body.Token = result.Token;
                    break;
                }
            case Enums.ProviderType.BehPardakht:
                {
                    response.JsonBody.jsonStr = new ExpandoObject();
                    response.JsonBody.jsonStr.url = result.IpgBaseUrl;
                    response.JsonBody.jsonStr.method = "POST";
                    response.JsonBody.jsonStr.@params = new ExpandoObject();
                    response.JsonBody.jsonStr.@params.RefId = result.Token;
                    response.JsonBody.jsonStr.@params.MobileNo = !string.IsNullOrEmpty(mobileNumber) ? $"98{mobileNumber.Remove(0, 1)}" : null;
                    break;
                }
            case Enums.ProviderType.Ayandeh:
                {
                    response.JsonBody.jsonStr = new ExpandoObject();
                    response.JsonBody.jsonStr.url = result.IpgBaseUrl;
                    response.JsonBody.jsonStr.method = "POST";
                    response.JsonBody.jsonStr.@params = new ExpandoObject();
                    response.JsonBody.jsonStr.@params.traceNumber = result.Token;
                    response.JsonBody.jsonStr.@params.username = result.UserName;
                    break;
                }
            default:
                break;
        }
        return Result<PaymentTokenResponseViewModel>.SuccessResult(response);
    }
}