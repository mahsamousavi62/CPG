using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Helper;
using CPG.Application.UseCases.DirectDebit.Queries;
using System.Text.Json;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Infrastructure.Persistence.QueryHandlers.DirectDebit;

public class GetWithdrawalRequestQueryHandler(
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
    ReadDbContext context) : IRequestHandler<GetWithdrawalRequestQuery, Result<bool>>
{
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

    public async Task<Result<bool>> Handle(GetWithdrawalRequestQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode), cancellationToken);
            if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();
            if (!paymentRequest.Company.IsActive) throw new PaymentTokenInactiveCompanyException();
            if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
            if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
            if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);

            var grant = await _grantRepository.GetBySpecAsync(new DirectDebitGrantByIdSpec((long)request.PaymentToken.GrantId), cancellationToken);
            if (grant.Status != Enums.DirectDebitGrantStatus.Activated) throw new PaymentRequestGrantStatusException();
            if (DateTime.Now.Date > grant.ExpirationDate.Date) throw new PaymentRequestGrantExpirationDateException();
            long destinationDepositId;
            Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit companyDeposit;
            //if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationDepositIban))
            //{
            //    companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(paymentRequest.DestinationDepositIban), cancellationToken);
            //    if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
            //    if (companyDeposit.CompanyId != paymentRequest.CompanyId) { throw new PaymentTokenDepositNotBelongsCompanyException(); }
            //}
            //else
            //{
                companyDeposit = await _companyDepositRepository.GetBySpecAsync(new DefaultDirectDebitDepositSpec(paymentRequest.CompanyId), cancellationToken);
                if (companyDeposit is null) throw new Exception("Default CompanyDeposit for DirectDebit not found!");
            //}

            if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
            var bank = grant.Bank;
            if (!bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
            var provider =  bank.DirectDebitSetting.Provider;
            if (!provider.IsActive) { throw new PaymentTokenInactiveProviderException(); }
            if (!bank.DirectDebitSetting.IsActive) { throw new PaymentRequestInactiveDirectDebitSettingException(); }
            if (provider.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false) { throw new PaymentRequestProviderHasNoDDMethodException(); }
            if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false) { throw new PaymentRequestCompanyHasNoDDMethodException(); }

            if (paymentRequest.Amount > bank.DirectDebitSetting.MaxWithdrawalAmountPerDay) throw new PaymentRequestAmountBankLimitException();
            if (paymentRequest.Company.NationalCodeMatchingRequied && bank.DirectDebitSetting.AuthenticationType != Enums.AuthenticationType.CheckMobileAndDepositOwnershipMatching) throw new PaymentRequestAuthenticationTypeException();
            if (provider.Id != grant.ProviderId) throw new PaymentRequestNotEqualProviderIdException();

            var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(grant.Id), cancellationToken);
            var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(grant.Id), cancellationToken);

            if (paymentRequest.Amount > grant.Bank.DirectDebitSetting.MaxWithdrawalAmountPerDay - currentDayTransactions.Sum(t => t.Amount)) throw new PaymentRequestDayTransactionsLimitException();
            if (grant.SuccessTransactionCountLimitPerMonth - currentMonthTransactions.Count() <= 0) throw new PaymentRequestMonthTransactionsLimitException();

            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);

            var directDebitProvider = _directDebitFactory.GetInstance(provider.ProviderType);

            var tokenResult = await directDebitProvider.GetTokenAsync(new TokenRequest { ProviderData = provider.ProviderData });
            var providerData = JObject.Parse(provider.ProviderData);
            if (providerData["Refresh_Token"].ToString() != tokenResult.RefreshToken)
            {
                providerData["Refresh_Token"] = tokenResult.RefreshToken;
                provider.ProviderData = Newtonsoft.Json.JsonConvert.SerializeObject(providerData);
                await _providerRepository.UpdateAsync(provider);
                await _providerRepository.SaveChangesAsync();
            }

            var withdrawRequest = new WithdrawalRequest
            {
                AccessToken = tokenResult.AccessToken,
                ProviderData = provider.ProviderData,
                Amount = paymentRequest.Amount,
                Description = "",
                GrantAuthorizationId = grant.AuthorizationId,
                IsInstant = true,
                MaxRetryCount = 16,
            };
            var result = await directDebitProvider.WithdrawAsync(withdrawRequest);

            destinationDepositId = companyDeposit.Id;

            var serializedData = JsonSerializer.Serialize(result.Data);
            var resultStatus = SharedServices.GetDirectDebitTransactionStatus(result.Data.Status);
            if (result.StatusCode == (short)HttpStatusCode.OK)
            {
                Transaction transaction = Transaction.Create(new CreateTransactionModel
                {
                    DestinationDepositId = destinationDepositId,
                    PaymentRequest = paymentRequest,
                    TransactionMethodType = Enums.TransactionType.DirectDebit,
                    Status = resultStatus == Enums.DirectDebitTransactionStatus.UnSuccessful ?
                        Enums.TransactionStatus.TransactionFailed : Enums.TransactionStatus.InPrgress,
                    DDTransactionModel = new CreateDDTransactionModel
                    {
                        GrantId = grant.Id,
                        Status = SharedServices.GetDirectDebitTransactionStatus(result.Data.Status),
                        TrackId = result.TrackerId,
                        ProviderTrackId = result.Data.Id,
                        ProviderData = serializedData
                    }
                });
                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();
                paymentRequest.IsUsed = true;
                paymentRequest.Status = resultStatus == Enums.DirectDebitTransactionStatus.TransactionSucceeded ? Enums.PaymentStatus.TransactionWaitingForVerification :
                    resultStatus == Enums.DirectDebitTransactionStatus.UnSuccessful ? Enums.PaymentStatus.TransactionFailed : Enums.PaymentStatus.InProgress;
                PaymentRequest.Update(paymentRequest);
                await _paymentRequestRepository.UpdateAsync(paymentRequest);
                await _paymentRequestRepository.SaveChangesAsync();
            }
            else
            {
                return Result<bool>.Failure(new Error("2009000", GlobalResource.GetPaymentTicketUnexpectedError));
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (DomainException exc)
        {
            return Result<bool>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<bool>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<bool>.Failure(new Error("2009000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }
}