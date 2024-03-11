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
using CPG.Application.UseCases.PaymentReceipt.Queries;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Application.UseCases.PaymentReceipt.ViewModels;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentReceipt;

public class AddPaymentReceiptQueryHandler(
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    ReadDbContext context) : IRequestHandler<AddPaymentReceiptQuery, Result<PaymentReceiptResponseViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> _companyDepositRepository = companyDepositRepository;

    public async Task<Result<PaymentReceiptResponseViewModel>> Handle(AddPaymentReceiptQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.viewModel.PaymentRequestCode), cancellationToken);
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
                companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositsByIdList(new List<long> { request.viewModel.CompanyDepositId }), cancellationToken);
                if (companyDeposit is null) throw new Exception("Default CompanyDeposit for DirectDebit not found!");
            }
            destinationDepositId = companyDeposit.Id;

            if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
            var bank = companyDeposit.Bank;
            if (!bank.IsActive) { throw new PaymentTokenInactiveBankException(); }            
            if (company.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.PaymentReceipt) is false) { throw new PaymentRequestCompanyHasNoReceiptMethodException(); }

            var transaction = Transaction.Create(new CreateTransactionModel
            {
                DestinationDepositId = destinationDepositId,
                PaymentRequest = paymentRequest,
                TransactionMethodType = Enums.TransactionType.PaymentReceipt,                
                Status = Enums.TransactionStatus.InPrgress,                
                PaymentReceiptModel = new PaymentReceiptTransactionModel
                {
                    Description = paymentRequest.Description,
                    Status = Enums.PaymentReceiptStatus.SucceededAndWaitingForVerification,
                    ReceiptDateTime = request.viewModel.SettlementDateTime,
                    ReceiptImage = new Logo (request.viewModel.File),
                    ReferenceNumber = request.viewModel.ReceiptIdentifier,
                    SourceIban = new Iban(request.viewModel.Iban)
                }
            });

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            paymentRequest.IsUsed = true;
            paymentRequest.Status = Enums.PaymentStatus.TransactionWaitingForVerification;
            PaymentRequest.Update(paymentRequest);
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();

            return Result<PaymentReceiptResponseViewModel>.SuccessResult(new PaymentReceiptResponseViewModel
            {
                CallbackUrl = $"{transaction.PaymentRequest.CallBackUrl}/paymentResult?paymentCode={transaction.PaymentRequest.PaymentCode}&paymentStatus={General.GetPaymentStatusTitle(transaction.PaymentRequest.Status)}"
            });
        }
        catch (DomainException exc)
        {
            return Result<PaymentReceiptResponseViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<PaymentReceiptResponseViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<PaymentReceiptResponseViewModel>.Failure(new Error("2009000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }
}