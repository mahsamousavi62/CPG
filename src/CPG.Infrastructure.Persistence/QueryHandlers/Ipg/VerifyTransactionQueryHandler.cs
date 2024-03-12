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
using CPG.Domain.SharedKernel.Communication.DirectDebit;

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

            if (paymentRequest.Status != PaymentStatus.TransactionWaitingForVerification &&
                paymentRequest.Status != PaymentStatus.TransactionVerificationFailed
                ) { throw new VerifyInvalidStatusException(); }

            paymentRequest.Status = PaymentStatus.TransactionVerifiedByApplication;
            paymentRequest.VerificationDateTime ??= DateTime.Now;

            await _paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByPaymentRequestId(paymentRequest.Id), cancellationToken);
            if (transaction is null || (transaction.IPGTransaction is null && transaction.DirectDebitTransaction is null))
            {
                throw new Exception("transaction or transactionDetail not found");
            }

            switch (transaction.TransactionMethodType)
            {
                case TransactionType.IPG:
                    {
                        var providerType = transaction.IPGTransaction.CompanyIPG.Provider.ProviderType;

                        var ipg = _ipgFactory.GetInstance(providerType);
                        var result = await ipg.Verify(new VerifyTransactionRequest
                        {
                            ProviderData = transaction.IPGTransaction.CompanyIPG.ProviderData,
                            ProviderTrackerId = transaction.IPGTransaction.ProviderTrackerId,
                            Token = transaction.IPGTransaction.IPGToken
                        });

                        transaction.IPGTransaction.Status = result.Status;

                        if (result.Status == IPGTransactionStatus.VerificationSucceeded)
                        {
                            var currentDateTime = DateTime.Now;
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
                        break;
                    }
                case TransactionType.DirectDebit:
                case TransactionType.PaymentReceipt:
                case TransactionType.CharismaCard:
                    {
                        var date = DateTime.Now.AddDays(1);
                        transaction.PredictedSettlementDateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                        transaction.Status = TransactionStatus.TransactionSucceeded;
                        paymentRequest.Status = PaymentStatus.TransactionVerificationSucceeded;

                        break;
                    }

                default:
                    break;
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
                ReferenceNumber = GetTransactionRefrenceNumber(transaction),
                PaymentMethodType = (short)transaction?.TransactionMethodType,
                PaymentMethodTypeTitle = transaction is null ? string.Empty : GetPaymentMethodTypeTitle(transaction.TransactionMethodType),
                Status = (short)paymentRequest.Status,
                StatusTitle = General.GetPaymentStatusTitle(paymentRequest.Status),
                PredictedSettlementDateTime = transaction.PredictedSettlementDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                CPGVerificationDateTime = GetTransactionVerificationDateTime(transaction),
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

    private string GetPaymentMethodTypeTitle(TransactionType type)
    {
        return type switch
        {
            TransactionType.IPG => "INTERNET_PAYMENT_GATEWAY",
            TransactionType.DirectDebit => "DIRECT_DEBIT",
            TransactionType.PaymentReceipt => "PAYMENT_RECEIPT",
            TransactionType.CharismaCard => "CHARISMA_CARD",
            _ => string.Empty
        };
    }

    private string GetTransactionRefrenceNumber(Transaction transaction)
    {
        switch (transaction.TransactionMethodType)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction.ReferenceNumber;
            case TransactionType.DirectDebit:
                return string.Empty;
            case TransactionType.PaymentReceipt:
            case TransactionType.CharismaCard:
                return transaction.PaymentReceiptTransaction.ReferenceNumber;
            default:
                return string.Empty;
        }
    }

    private string GetTransactionVerificationDateTime(Transaction transaction)
    {
        switch (transaction.TransactionMethodType)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction.VerificationDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz");
            case TransactionType.DirectDebit:
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            case TransactionType.PaymentReceipt:
            case TransactionType.CharismaCard:
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");

            default:
                return string.Empty;
        }
    }
}