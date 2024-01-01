using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Ipg.ViewModels;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class VerifyTransactionQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    ReadDbContext context) : IRequestHandler<VerifyTransactionQuery, ResultData<VerifyTransactionResponseViewModel>>
{

    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<VerifyTransactionResponseViewModel>> Handle(VerifyTransactionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == request.VerifyTransaction.CompanyIPGId);
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.VerifyTransaction.CompanyIPGId); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.Verify(new VerifyTransactionRequest
            {
                ProviderData = companyIpg.ProviderData,
                ProviderTrackerId = 1,
            });

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByProviderTrackerId(request.VerifyTransaction.ProviderTrackerId.ToString()));
            var paymentRequest = await _paymentRequestRepository.GetByIdAsync(transaction.PaymentRquestId);

            if (transaction is null || paymentRequest is null || transaction.IPGTransaction is null)
            {
                return new ResultData<VerifyTransactionResponseViewModel>
                {
                    OperationResult = Enums.OperationResult.Failed,
                    Error = GlobalResource.UnexpectedError
                };
            }

            transaction.IPGTransaction.Status = result.Status;
            //ToDo:
            //transaction.IPGTransaction.VerificationDateTime = ;

            if (result.Status == Enums.IPGTransactionStatus.VerificationSucceeded)
            {
                var currentDateTime = DateTime.UtcNow.Date;
                var timeMargin = new TimeOnly(22, 45);
                var currentTime = new TimeOnly(currentDateTime.Hour, currentDateTime.Minute);
                var date = DateTime.UtcNow.DayOfWeek == DayOfWeek.Thursday ?
                    new DateTime(currentDateTime.AddDays(2).Year, currentDateTime.AddDays(2).Month, currentDateTime.AddDays(2).Day, currentTime < timeMargin ? 11 : 22, 45, 0) :
                    new DateTime(currentDateTime.AddDays(1).Year, currentDateTime.AddDays(1).Month, currentDateTime.AddDays(1).Day, currentTime < timeMargin ? 11 : 22, 45, 0);

                transaction.PredictedSettlementDateTime = date;
                if (result.Status == Enums.IPGTransactionStatus.VerificationSucceeded)
                {
                    transaction.Status = Enums.TransactionStatus.TransactionSucceeded;
                    paymentRequest.Status = Enums.PaymentStatus.TransactionVerificationSucceeded;
                }
                else if(result.Status == Enums.IPGTransactionStatus.VerificationFailed)
                {
                    transaction.Status = Enums.TransactionStatus.TransactionFailed;
                    paymentRequest.Status = Enums.PaymentStatus.TransactionVerificationFailed;
                }
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();

            var response = new VerifyTransactionResponseViewModel
            {
                Amount = paymentRequest.Amount.ToString(),
                Code = paymentRequest.Code,
                TrackerId = paymentRequest.TrackerId,
                DestinationDepositIban = transaction.DestinationDeposit.Iban,
                ReferenceNumber = transaction.IPGTransaction.ReferenceNumber,
                PaymentMethodType = ((short)transaction?.TransactionMethodType).ToString(),
                PaymentMethodTypeTitle = transaction is null ? string.Empty : GetPaymentMethodTypeTitle(transaction.TransactionMethodType),
                Status = ((short)paymentRequest.Status).ToString(),
                StatusTitle = GetStatusTitle(paymentRequest.Status),
                PredictedExpirationDateTime = transaction.PredictedSettlementDateTime.ToString(),
                //ToDo:
                //CPGVerificationDateTime = transaction.IPGTransaction.VerificationDateTime
                CPGVerificationDateTime = string.Empty
            };

            return new ResultData<VerifyTransactionResponseViewModel>
            {
                OperationResult = Enums.OperationResult.Succeeded,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ResultData<VerifyTransactionResponseViewModel>
            {
                OperationResult = Enums.OperationResult.Failed,
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