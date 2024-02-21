using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.Ipg.Exception;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
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
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> companyIPGRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    IAuthenticationService authenticationService,
    ReadDbContext context) : IRequestHandler<GetPaymentTokenCommand, Result<PaymentTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> _companyIPGRepository = companyIPGRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _grantRepository = grantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<PaymentTokenResponseViewModel>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode), cancellationToken);
            if (paymentRequest is null) { throw new PaymentRequestNotFoundByCodeException(); }
            if (!paymentRequest.Company.IsActive) { throw new PaymentTokenInactiveCompanyException(); }
            if (paymentRequest.UrlExpirationDateTime < DateTime.Now) { throw new PaymentRequestCodeExpiredException(); }
            if (paymentRequest.IsUsed) { throw new PaymentRequestCodeIsUsedBeforeException(); }
            if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) { throw new PaymentRequestCodeInvalidStatusException(); }

            if (paymentRequest.Company.NationalCodeMatchingRequied is true &&
                (string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Iv) || string.IsNullOrEmpty(paymentRequest.Company.ShaparakSetting?.Key)))
            {
                throw new PaymentTokenNullKeyOrIvException();
            }

            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);
            if (request.PaymentToken.CompanyIPGId is not null)
            {
                var companyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGIncludeProvider((long)request.PaymentToken.CompanyIPGId), cancellationToken);
                if (companyIpg is null) { throw new CompanyIPGNotFoundException((long)request.PaymentToken.CompanyIPGId); }
                if (!companyIpg.IsActive) { throw new PaymentTokenInactiveIPGException(); }
                if (!companyIpg.IPGType.IsActive) { throw new PaymentTokenInactiveIPGTypeException(); }
                if (!companyIpg.Provider.IsActive) { throw new PaymentTokenInactiveProviderException(); }


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

                if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
                if (!companyDeposit.Bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
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
                var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);

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

                }
                else
                {
                    return Result<PaymentTokenResponseViewModel>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
                }

                return CreateResponseModel(paymentRequest, mobileNumber, result, companyIpg);
            }
            else if (request.PaymentToken.GrantId is not null)
            {
                var grant = await _grantRepository.GetBySpecAsync(new DirectDebitGrantByIdSpec((long)request.PaymentToken.GrantId), cancellationToken);
                if (grant.Status != Enums.DirectDebitGrantStatus.Activated) throw new PaymentRequestGrantStatusException();
                if (DateTime.Now.Date > grant.ExpirationDate.Date) throw new PaymentRequestGrantExpirationDateException();
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
                    companyDeposit = await _companyDepositRepository.GetBySpecAsync(new DefaultDirectDebitDepositSpec(paymentRequest.CompanyId), cancellationToken);
                    if (companyDeposit is null) throw new Exception("Default CompanyDeposit for DirectDebit not found!");
                }

                if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
                if (!companyDeposit.Bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
                var bank = await _bankRepository.GetBySpecAsync(new BankByIdSpec(companyDeposit.BankId), cancellationToken);
                if (!bank.DirectDebitSetting.Provider.IsActive) { throw new PaymentTokenInactiveProviderException(); }
                if (!bank.DirectDebitSetting.IsActive) { throw new PaymentRequestInactiveDirectDebitSettingException(); }
                if (bank.DirectDebitSetting.Provider.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false) { throw new PaymentRequestProviderHasNoDDMethodException(); }
                if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false) { throw new PaymentRequestCompanyHasNoDDMethodException(); }

                if (paymentRequest.Amount > bank.DirectDebitSetting.MaxWithdrawalAmountPerDay) throw new PaymentRequestAmountBankLimitException();
                if (paymentRequest.Company.NationalCodeMatchingRequied && bank.DirectDebitSetting.AuthenticationType != Enums.AuthenticationType.CheckMobileAndDepositOwnershipMatching) throw new PaymentRequestAuthenticationTypeException();
                if (bank.DirectDebitSetting.ProviderId != grant.ProviderId) throw new PaymentRequestNotEqualProviderIdException();

                var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(grant.Id), cancellationToken);
                var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(grant.Id), cancellationToken);

                if (paymentRequest.Amount > grant.Bank.DirectDebitSetting.MaxWithdrawalAmountPerDay - currentDayTransactions.Sum(t => t.Amount)) throw new PaymentRequestNotEqualProviderIdException();
                if (grant.SuccessTransactionCountLimitPerMonth - currentMonthTransactions.Count() <= 0) throw new PaymentRequestNotEqualProviderIdException();

                var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);

                return CreateResponseModel(paymentRequest, mobileNumber, null, null, grant);


                destinationDepositId = companyDeposit.Id;
            }
            return Result<PaymentTokenResponseViewModel>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));

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

    private static Result<PaymentTokenResponseViewModel> CreateResponseModel(PaymentRequest paymentRequest, string mobileNumber, PaymentTokenResponse result,
        Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG companyIpg = null, DirectDebitGrant directDebitGrant = null)
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



