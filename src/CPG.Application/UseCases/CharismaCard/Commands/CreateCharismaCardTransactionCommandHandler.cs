
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
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;
using CPG.Domain.SharedKernel.Helper;
using static CPG.Domain.SharedKernel.Enums;
using System.Linq;
using System.Text.RegularExpressions;
using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Application.Shared.Resource;

namespace CPG.Application.UseCases.CharismaCard.Commands;

public class CreateCharismaCardTransactionCommandHandler(
    INeoBankService neoBankService,
    IAuthenticationService authenticationService,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository
    )
    : IRequestHandler<CreateCharismaCardTransactionCommand, Result<CharismaCardResponseViewModel>>
{
    private readonly INeoBankService neoBankService = neoBankService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    public async Task<Result<CharismaCardResponseViewModel>> Handle(CreateCharismaCardTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.Model.PaymentCode), cancellationToken);
        if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();
        if (!paymentRequest.Company.IsActive) throw new PaymentTokenInactiveCompanyException();
        if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
        if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
        if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

        var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);

        long destinationDepositId;
        Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit companyDeposit;

        companyDeposit = await _companyDepositRepository.GetBySpecAsync(new DefaultCharismaCardDepositSpec(paymentRequest.CompanyId), cancellationToken);
        if (companyDeposit is null) throw new Exception("Default CompanyDeposit for charismCard not found!");

        if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
        var bank = companyDeposit.Bank;
        if (!bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
        if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.CharismaCard) is false)
        { throw new PaymentRequestCompanyHasNoCharismaCardMethodException(); }

        var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
        destinationDepositId = companyDeposit.Id;


        var trackId = RandomGenerator.GenerateRandomDigitNumber(16);
        var accountNumber = Regex.Replace(companyDeposit.AccountNumber, @"(\d{4})(\d{2})(\d{3})(\d+)", "$1/$2/$3/$4");
        var clientDirectDebitResponse = await neoBankService.ClientDirectDebit(new ClientDirectDebitRequest
        {
            Amount = paymentRequest.Amount,
            Description = paymentRequest.Description,
            DestinationDepositNumber = accountNumber,
            TrackerId = trackId,
        });

        if (clientDirectDebitResponse?.IsSuccess == true)
        {
            ClientDirectDebitResponse clientDirectDebit = clientDirectDebitResponse.Data;
            if (!string.IsNullOrEmpty(clientDirectDebitResponse?.Data?.ErrorCode))
            {
                if (clientDirectDebit.ErrorCode == "10")
                {
                    return Result<CharismaCardResponseViewModel>.Failure(new Error("2202005", string.Format(GlobalResource.DirectDebitGrantError, company.PersianName)));
                }
            }
            var CharismaCardstatus = clientDirectDebit.TransferStatus == NeoBankTransferStatus.Failed ?
                CharismaCardStatus.Failed : CharismaCardStatus.Done;
            Transaction transaction = Transaction.Create(new CreateTransactionModel
            {
                CharismaCardModel = new CharismaCardTransactionModel
                {
                    TrackId = trackId,
                    ProviderTrackId = clientDirectDebit.TranactionId ?? "0",
                    ReferenceNumber = clientDirectDebit.ReferenceNumber,
                    Status = CharismaCardstatus
                },

                DestinationDepositId = destinationDepositId,
                PaymentRequest = paymentRequest,
                TransactionMethodType = Enums.TransactionType.CharismaCard,
                Status = CharismaCardstatus == CharismaCardStatus.Done ?
                Enums.TransactionStatus.TransactionSucceeded :
                Enums.TransactionStatus.TransactionFailed

            });
            await transactionRepository.AddAsync(transaction);
            await transactionRepository.SaveChangesAsync();

            switch (CharismaCardstatus)
            {
                case CharismaCardStatus.Done:
                    paymentRequest.Status = Enums.PaymentStatus.TransactionWaitingForVerification;
                    transaction.PredictedSettlementDateTime = transaction.CharismaCardTransaction.CreationDate;
                    break;
                case CharismaCardStatus.Failed:
                    paymentRequest.Status = Enums.PaymentStatus.TransactionFailed;
                    break;
                default:
                    break;
            }
            paymentRequest.IsUsed = true;
            PaymentRequest.Update(paymentRequest);
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();
            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return Result<CharismaCardResponseViewModel>.SuccessResult(new CharismaCardResponseViewModel
            {
                CallBackUrl = $"{transaction.PaymentRequest.CallBackUrl}/paymentResult?paymentCode={transaction.PaymentRequest.PaymentCode}&paymentStatus={General.GetPaymentStatusTitle(transaction.PaymentRequest.Status)}"
            });
        }
        else
        {
            return Result<CharismaCardResponseViewModel>.Failure(new Error(clientDirectDebitResponse.Error.Code, clientDirectDebitResponse.Error.Description));
        }
    }
}
