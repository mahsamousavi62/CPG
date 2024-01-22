using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.ViewModels;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class VerifyTransactionQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IHttpContextAccessor httpContext) : IRequestHandler<VerifyTransactionQuery, Result<VerifyTransactionResponseViewModel>>
{

    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    public async Task<Result<VerifyTransactionResponseViewModel>> Handle(VerifyTransactionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.VerifyTransaction.Code) && string.IsNullOrEmpty(request.VerifyTransaction.TrackerId))
            {
                throw new VerifyRequiredCodeOrTrackIdException();
            }

            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCodeOrTrackerId(request.VerifyTransaction.Code,
                request.VerifyTransaction.TrackerId), cancellationToken) ?? throw new VerifyInvalidCodeOrTrackIdException();

            _ = long.TryParse(_httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value, out long applicationId);

            if (paymentRequest.ApplicationId != applicationId) { throw new VerifyInvalidApplicationException(); }

            if (paymentRequest.Status != PaymentStatus.TransactionWaitingForVerification ||
                paymentRequest.Status != PaymentStatus.TransactionVerificationFailed
                ) { throw new VerifyInvalidStatusException(); }

            paymentRequest.Status = PaymentStatus.TransactionVerifiedByApplication;
            paymentRequest.VerificationDateTime ??= DateTime.Now;   

            await _paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByPaymentRequestId(paymentRequest.Id), cancellationToken);
            if (transaction is null || transaction.IPGTransaction is null)
            {
                throw new Exception("transaction or ipgTransaction not found");
            }

            var providerType = transaction.IPGTransaction.CompanyIPG.Provider.ProviderType;

            var ipg = _ipgFactory.GetInstance(providerType);
            var result = await ipg.Verify(new VerifyTransactionRequest
            {
                ProviderData = transaction.IPGTransaction.CompanyIPG.ProviderData,
                ProviderTrackerId = transaction.IPGTransaction.ProviderTrackerId,
            });

            transaction.IPGTransaction.Status = result.Status;

            if (result.Status == IPGTransactionStatus.VerificationSucceeded)
            {
                var currentDateTime = DateTime.UtcNow.Date;
                var timeMargin = new TimeOnly(23, 45);
                var currentTime = new TimeOnly(currentDateTime.Hour, currentDateTime.Minute);
                var date = currentTime < timeMargin ?
                    new DateTime(currentDateTime.AddDays(1).Year, currentDateTime.AddDays(1).Month, currentDateTime.AddDays(1).Day, 7, 0, 0) :
                    new DateTime(currentDateTime.AddDays(2).Year, currentDateTime.AddDays(2).Month, currentDateTime.AddDays(2).Day, 7, 0, 0);

                transaction.PredictedSettlementDateTime = date;
                transaction.Status = TransactionStatus.TransactionSucceeded;
                transaction.IPGTransaction.VerificationDateTime = DateTime.Now;
                paymentRequest.Status = PaymentStatus.TransactionVerificationSucceeded;
               
            }
            else if (result.Status == IPGTransactionStatus.VerificationFailed)
            {
                transaction.Status = TransactionStatus.TransactionFailed;
                paymentRequest.Status = PaymentStatus.TransactionVerificationFailed;
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();

            var response = new VerifyTransactionResponseViewModel
            {
                Amount = paymentRequest.Amount,
                Code = paymentRequest.PaymentCode,
                TrackerId = paymentRequest.TrackerId,
                DestinationDepositIban = transaction.DestinationDeposit.Iban,
                DestinationDepositAccountNumber = transaction?.DestinationDeposit?.AccountNumber,
                ReferenceNumber = transaction.IPGTransaction.ReferenceNumber,
                PaymentMethodType = (short)transaction?.TransactionMethodType,
                PaymentMethodTypeTitle = transaction is null ? string.Empty : GetPaymentMethodTypeTitle(transaction.TransactionMethodType),
                Status = (short)paymentRequest.Status,
                StatusTitle = GetStatusTitle(paymentRequest.Status),
                PredictedSettlementDateTime = transaction.PredictedSettlementDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                CPGVerificationDateTime = transaction.IPGTransaction.VerificationDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz"),
            };

            return Result<VerifyTransactionResponseViewModel>.SuccessResult(response);
        }
        catch (DomainException exc)
        {
            return Result<VerifyTransactionResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<VerifyTransactionResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<VerifyTransactionResponseViewModel>.Failure(new Error("1005000", GlobalResource.TransactionDetailUnexpectedError));
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
            PaymentStatus.TransactionCancellationSucceeded => "TRANSACTION_CANCELLATION_SUCCEEDED",
            PaymentStatus.TransactionCancellationFailed => "TRANSACTION_CANCELLATION_FAILED",
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