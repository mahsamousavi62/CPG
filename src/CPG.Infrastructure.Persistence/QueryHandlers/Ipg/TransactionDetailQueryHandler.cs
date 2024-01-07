using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Application.Shared.Resource;
using CPG.Domain.Exceptions;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class TransactionDetailQueryHandler(IAggregateRepository<Transaction> transactionRepository, ReadDbContext context) : IRequestHandler<TransactionDetailQuery, Result<TransactionDetailResponseViewModel>>
{
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ReadDbContext _context = context;

    public async Task<Result<TransactionDetailResponseViewModel>> Handle(TransactionDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RequestViewModel.Code) && string.IsNullOrEmpty(request.RequestViewModel.TrackerId))
            {
                throw new RequiredCodeOrTrackIdException();
            }
            var paymentRequest = await _context.PaymentRequestReadModels.FirstOrDefaultAsync(t => t.PaymentCode == request.RequestViewModel.Code ||
                                                                                                  t.TrackerId == request.RequestViewModel.TrackerId);

            if (paymentRequest is null) { throw new InvalidCodeOrTrackIdException(); }

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByPaymentRequestId(paymentRequest.Id));

            return Result<TransactionDetailResponseViewModel>.SuccessResult(
                new TransactionDetailResponseViewModel
                {
                    Code = paymentRequest.PaymentCode,
                    TrackerId = paymentRequest.TrackerId,
                    Amount = paymentRequest.Amount,
                    Status = (short)paymentRequest.Status,
                    StatusTitle = GetStatusTitle(paymentRequest.Status),
                    PaymentMethodType = (short)transaction?.TransactionMethodType,
                    PaymentMethodTypeTitle = transaction is null ? string.Empty : GetPaymentMethodTypeTitle(transaction.TransactionMethodType),
                    ReferenceNumber = transaction is not null && transaction.TransactionMethodType == TransactionType.IPG ? transaction.IPGTransaction?.ReferenceNumber : string.Empty,
                    DestinationDepositIban = transaction?.DestinationDeposit?.Iban,
                    PredictedExpirationDateTime = transaction is not null && transaction.TransactionMethodType == TransactionType.IPG ? transaction.IPGTransaction?.PredicateExpirationDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz") : string.Empty,
                });
        }
        catch (Exception exc)
        {
            if (exc is DomainException || exc is CPG.Application.UseCases.Exceptions.ApplicationException)
                return Result<TransactionDetailResponseViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
            else
                return Result<TransactionDetailResponseViewModel>.Failure(new Error("1002000", GlobalResource.TransactionDetailUnexpectedError));
        }
    }

    private string GetStatusTitle(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Draft => "DRAFT",
            PaymentStatus.RedirectedToCpg => "REDIRECTED_TO_CPG",
            PaymentStatus.CanceledByUser => "CANCELLED_BY_USER",
            PaymentStatus.InProgress => "TRANSACTION_IN_PROGRESS",
            PaymentStatus.TransactionWaitingForVerification => "TRANSACTION_WAITING_FOR_VERIFICATION",
            PaymentStatus.TransactionFailed => "TRANSACTION_FAILED",
            PaymentStatus.TransactionVerifiedByApplication => "TRANSACTION_VERIFIED_BY_APPLICATION",
            PaymentStatus.TransactionCanceledByApplication => "TRANSACTION_CANCELLED_BY_APPLICATION",
            PaymentStatus.TransactionVerificationSucceeded => "TRANSACTION_VERIFICATION_SUCCEEDED",
            PaymentStatus.TransactionVerificationFailed => "TRANSACTION_VERIFICATION_FAILED",
            PaymentStatus.SettlementSucceeded => "SETTLEMENT_SUCCEEDED",
            PaymentStatus.SettlementFailed => "SETTLEMENT_FAILED",
            _ => string.Empty
        };
    }

    private string GetPaymentMethodTypeTitle(TransactionType type)
    {
        return type switch
        {
            TransactionType.IPG => "INTERNET_PAYMENT_GATEWAY",
            TransactionType.DirectDebit => "DIRECT_DEBIT",
            _ => string.Empty
        };
    }
}