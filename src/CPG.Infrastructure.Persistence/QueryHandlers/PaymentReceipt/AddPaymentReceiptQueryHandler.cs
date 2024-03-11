using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Linq;
using System.Security.Claims;
using CPG.Application.UseCases.PaymentReceipt.Queries;
using System.Threading.Tasks;
using System.Threading;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentReceipt;

public class AddPaymentReceiptQueryHandler(
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository,
    IAuthenticationService authenticationService,
    ReadDbContext context) : IRequestHandler<AddPaymentReceiptQuery, Result<bool>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;    
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<bool>> Handle(AddPaymentReceiptQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.viewModel.PaymentRequestCode), cancellationToken);
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
            var bank = companyDeposit.Bank;
            if (!bank.IsActive) { throw new PaymentTokenInactiveBankException(); }            
            if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false) { throw new PaymentRequestCompanyHasNoDDMethodException(); }

            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
            destinationDepositId = companyDeposit.Id;

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