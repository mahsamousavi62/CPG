
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;
using CPG.Domain.SharedKernel.Helper;
using static CPG.Domain.SharedKernel.Enums;
using System.Linq;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CreateCharismaCardTransactionCommandHandler(INeoBankService neoBankService,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository,
    IAuthenticationService authenticationService)
    : IRequestHandler<CreateCharismaCardTransactionCommand, Result<Unit>>
{
    private readonly INeoBankService neoBankService = neoBankService;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    public async Task<Result<Unit>> Handle(CreateCharismaCardTransactionCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.Model.PaymentRequestCode), cancellationToken);
        if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();
        if (!paymentRequest.Company.IsActive) throw new PaymentTokenInactiveCompanyException();
        if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
        if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
        if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();
        
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

        var trackId = RandomGenerator.GenerateRandomDigitNumber(16);
        var clientDirectDebitResponse = await neoBankService.ClientDirectDebit(new ClientDirectDebitRequest
        {
            Amount = paymentRequest.Amount,
            Description = paymentRequest.Description,
            DestinationDepositNumber = companyDeposit.Iban,
            TrackerId = trackId,
        });

        if (clientDirectDebitResponse.IsSuccess)
        {
            ClientDirectDebitResponse clientDirectDebit = clientDirectDebitResponse.Data;
            CharismaCardStatus status = CharismaCardStatus.Done;
            if (clientDirectDebit.TransferStatus == NeoBankTransferStatus.Failed)
            {
                status = CharismaCardStatus.Failed;
            }
            Transaction transaction = Transaction.Create(new CreateTransactionModel
            {
                DestinationDepositId = destinationDepositId,
                PaymentRequest = paymentRequest,
                TransactionMethodType = Enums.TransactionType.CharismaCard,
                Status = Enums.TransactionStatus.InPrgress,

                CharismaCardModel = new CharismaCardTransactionModel
                {
                    TrackId = trackId,
                    ProviderTrackId = clientDirectDebit.TranactionId,
                    ReferenceNumber = clientDirectDebit.ReferenceNumber,
                    Status = status
                }
            });
            await transactionRepository.AddAsync(transaction);
            await transactionRepository.SaveChangesAsync();
            return Result<Unit>.SuccessResult(Unit.Value);
        }
        else
        {
            return Result<Unit>.Failure(new Error("", ""));
        }
    }
}
