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

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class TransactionDetailQueryHandler(IAggregateRepository<Transaction> transactionRepository, ReadDbContext context) : IRequestHandler<TransactionDetailQuery, ResultData<TransactionDetailResponseViewModel>>
{
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<TransactionDetailResponseViewModel>> Handle(TransactionDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RequestViewModel.Code) && string.IsNullOrEmpty(request.RequestViewModel.TrackerId))
            {
                throw new RequiredCodeOrTrackIdException(string.Empty);
            }
            var paymentRequest = await _context.PaymentRequestReadModels.FirstOrDefaultAsync(t => t.Code == request.RequestViewModel.Code ||
                                                                                                  t.TrackerId == request.RequestViewModel.TrackerId);

            if (paymentRequest is null) { throw new InvalidCodeOrTrackIdException(string.Empty); }

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByPaymentRequestId(paymentRequest.Id));

            return new ResultData<TransactionDetailResponseViewModel>
            {
                OperationResult = OperationResult.Succeeded,
                Data = new TransactionDetailResponseViewModel
                {
                    Code = paymentRequest.Code,
                    TrackerId = paymentRequest.TrackerId,
                    Amount = paymentRequest.Amount.ToString(),
                    Status = ((short)paymentRequest.Status).ToString(),
                    StatusTitle = GetStatusTitle(paymentRequest.Status),
                    PaymentMethodType = ((short)transaction?.TransactionMethodType).ToString(),
                    PaymentMethodTypeTitle = transaction is null ? string.Empty : GetPaymentMethodTypeTitle(transaction.TransactionMethodType),
                    ReferenceNumber = transaction is not null && transaction.TransactionMethodType == TransactionType.IPG ? transaction.IPGTransaction?.ReferenceNumber : string.Empty,
                    DestinationDepositIban = transaction?.DestinationDeposit?.Iban,
                    PredictedExpirationDateTime = transaction is not null && transaction.TransactionMethodType == TransactionType.IPG ? transaction.IPGTransaction?.PredicateDateTime.ToString() : string.Empty,
                },
            };
        }
        catch (Exception ex)
        {
            return new ResultData<TransactionDetailResponseViewModel>
            {
                OperationResult = OperationResult.Failed,
                Error = ex.Message
            };
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