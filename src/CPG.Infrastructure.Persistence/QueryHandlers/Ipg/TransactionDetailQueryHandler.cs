using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class TransactionDetailQueryHandler(IAggregateRepository<Transaction> transactionRepository, ReadDbContext context, IHttpContextAccessor httpContext) : IRequestHandler<TransactionDetailQuery, Result<TransactionDetailResponseViewModel>>
{
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ReadDbContext _context = context;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    public async Task<Result<TransactionDetailResponseViewModel>> Handle(TransactionDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RequestViewModel.Code) && string.IsNullOrEmpty(request.RequestViewModel.TrackerId))
            {
                throw new RequiredCodeOrTrackIdException();
            }
            var paymentRequest = await _context.PaymentRequestReadModels.FirstOrDefaultAsync(t => t.PaymentCode == request.RequestViewModel.Code ||
                                                                                                  t.TrackerId == request.RequestViewModel.TrackerId,
                                                                                                  cancellationToken: cancellationToken) ?? throw new InvalidCodeOrTrackIdException();

            _ = long.TryParse(_httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);

            if (paymentRequest.ApplicationId != applicationId) { throw new TransactionDetailInvalidApplicationException(applicationId); }

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
                    ReferenceNumber = transaction is null ? string.Empty : GetTransactionRefrenceNumber(transaction.TransactionMethodType, transaction),
                    DestinationDepositIban = transaction?.DestinationDeposit?.Iban,
                    DestinationDepositAccountNumber = transaction?.DestinationDeposit?.AccountNumber,
                    PredictedExpirationDateTime = transaction is null ? string.Empty : GetTransactionPredictedExpirationDateTime(transaction.TransactionMethodType, transaction),
                });
        }
        catch (DomainException exc)
        {
            return Result<TransactionDetailResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<TransactionDetailResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
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

    private string GetTransactionRefrenceNumber(TransactionType type, Transaction transaction)
    {
        switch (type)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction.ReferenceNumber;
            case TransactionType.DirectDebit:
                return string.Empty;
            default:
                return string.Empty;
        }
    }

    private string GetTransactionPredictedExpirationDateTime(TransactionType type, Transaction transaction)
    {
        switch (type)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction?.PredicateExpirationDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz");
            case TransactionType.DirectDebit:
                return string.Empty;
            default:
                return string.Empty;
        }
    }
}