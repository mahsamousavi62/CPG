using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentReceipt.Queries;
using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentReceipt;

public class AddPaymentReceiptQueryHandler(
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IMinioProvider minioProvider) : IRequestHandler<AddPaymentReceiptQuery, Result<PaymentReceiptResponseViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;

    public async Task<Result<PaymentReceiptResponseViewModel>> Handle(AddPaymentReceiptQuery request, CancellationToken cancellationToken)
    {
        PaymentRequest paymentRequest = await _paymentRequestRepository
                .FirstOrDefaultAsync(new PaymentRequestByCode(request.viewModel.PaymentRequestCode), cancellationToken)
                ?? throw new PaymentRequestNotFoundByCodeException();

        if (!paymentRequest.Company.IsActive) throw new PaymentTokenInactiveCompanyException();
        if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();
        if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
        if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();

        var company = await _companyRepository.FirstOrDefaultAsync(new CompanyByIdSpec(paymentRequest.CompanyId), cancellationToken);

        long destinationDepositId;
        Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit companyDeposit;
        if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationDepositIban))
        {
            companyDeposit = await _companyDepositRepository
                .FirstOrDefaultAsync(new CompanyDepositByIban(paymentRequest.DestinationDepositIban), cancellationToken);

            if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
            if (companyDeposit.CompanyId != paymentRequest.CompanyId)
            {
                throw new PaymentTokenDepositNotBelongsCompanyException();
            }
        }
        else
        {
            companyDeposit = await _companyDepositRepository.FirstOrDefaultAsync(new CompanyDepositsByIdList([request.viewModel.CompanyDepositId]), cancellationToken);
            if (companyDeposit is null) throw new Exception("Default CompanyDeposit for DirectDebit not found!");
        }
        destinationDepositId = companyDeposit.Id;

        if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }

        var bank = companyDeposit.Bank;

        if (!bank.IsActive)
        {
            throw new PaymentTokenInactiveBankException();
        }

        if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.PaymentReceipt) is false)
        {
            throw new PaymentRequestCompanyHasNoReceiptMethodException();
        }

        var extention = Path.GetExtension(request.viewModel.File.FileName);
        request.viewModel.File.FileName = $"{request.viewModel.PaymentRequestCode}{extention}";
        Logo image = new(request.viewModel.File, Enums.UploadFromEntityType.PaymentReceipt.ToString(), _minioProvider);

        var transaction = Transaction.Create(new CreateTransactionModel
        {
            DestinationDepositId = destinationDepositId,
            PaymentRequest = paymentRequest,
            TransactionMethodType = Enums.TransactionType.PaymentReceipt,
            Status = Enums.TransactionStatus.InPrgress,
            PredictedSettlementDateTime = request.viewModel.SettlementDateTime,
            PaymentReceiptModel = new PaymentReceiptTransactionModel
            {
                Description = paymentRequest.Description,
                Status = Enums.PaymentReceiptStatus.SucceededAndWaitingForVerification,
                ReceiptDateTime = request.viewModel.SettlementDateTime,
                ReceiptImage = image,
                ReferenceNumber = request.viewModel.ReceiptIdentifier,
                SourceIban = new Iban(request.viewModel.Iban)
            }
        });

        _ = await _transactionRepository.AddAsync(transaction, cancellationToken);
        _ = await _transactionRepository.SaveChangesAsync(cancellationToken);

        paymentRequest.IsUsed = true;
        paymentRequest.Status = Enums.PaymentStatus.TransactionWaitingForVerification;
        PaymentRequest.Update(paymentRequest);

        await _paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
        _ = await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

        return Result<PaymentReceiptResponseViewModel>.SuccessResult(new PaymentReceiptResponseViewModel
        {
            CallbackUrl = transaction.PaymentRequest.CallBackUrl.Contains("?") ? $"{transaction.PaymentRequest.CallBackUrl.Split("?")[0]}/paymentResult?{transaction.PaymentRequest.CallBackUrl.Split("?")[1]}&paymentCode={transaction.PaymentRequest.PaymentCode}&paymentStatus={General.GetPaymentStatusTitle(transaction.PaymentRequest.Status)}" : $"{transaction.PaymentRequest.CallBackUrl}/paymentResult?paymentCode={transaction.PaymentRequest.PaymentCode}&paymentStatus={General.GetPaymentStatusTitle(transaction.PaymentRequest.Status)}"
        });
    }
}