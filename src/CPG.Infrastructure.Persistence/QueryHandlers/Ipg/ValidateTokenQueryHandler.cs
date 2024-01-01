using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.TransactionAggregate;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CPG.Application.UseCases.Ipg.ViewModels;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class ValidateTokenQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Transaction> transactionRepository,
    ReadDbContext context) : IRequestHandler<ValidateTokenQuery, ResultData<ValidateTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<ValidateTokenResponseViewModel>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByIPGTrackId(request.ValidateToken.TrackId));

        if (transaction?.IPGTransaction is null)
            throw new NotFoundTrackIdException();
        if (transaction.IPGTransaction.Status != IPGTransactionStatus.WaitingForPspResponse)
            throw new TrackIdInvalidStatusException();

        var paymentRequest = await _paymentRequestRepository.GetByIdAsync(transaction.PaymentRquestId);
        try
        {
            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == transaction.IPGTransaction.CompanyIPGId);
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(companyIpg.Id); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.GetTransactionResult(new TransactionResultRequest
            {
                ProviderData = companyIpg.ProviderData,
                LocalInvoiceId = transaction.IPGTransaction.TrackId,
            });

            switch (result.Status)
            {
                case 1:
                    {
                        transaction.IPGTransaction.Status = IPGTransactionStatus.FetchingResult;
                        break;
                    }
                case 2:
                    {
                        transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                        transaction.IPGTransaction.PredicateDateTime = result.PayGateTranDate.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                        paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                        break;
                    }
                case 3:
                    {
                        transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                        transaction.Status = TransactionStatus.TransactionFailed;
                        paymentRequest.Status = PaymentStatus.TransactionFailed;
                        break;
                    }
                default:
                    break;
            }

            transaction.IPGTransaction.ProviderTrackerId = result.PayGateTranID;
            transaction.IPGTransaction.ReferenceNumber = result.Rrn;
            transaction.IPGTransaction.EncryptCardNumber = result.Hash;

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _transactionRepository.SaveChangesAsync();

            return new ResultData<ValidateTokenResponseViewModel>
            {
                OperationResult = OperationResult.Succeeded,
                Data = new ValidateTokenResponseViewModel { CallbackUrl = $"{paymentRequest.CallBackUrl}/payment_result?code={paymentRequest.Code}&status={GetStatusTitle(paymentRequest.Status)}" },
            };
        }
        catch (Exception ex)
        {
            return new ResultData<ValidateTokenResponseViewModel>
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
            PaymentStatus.TransactionCancellationSucceeded => "TRANSACTION_CANCELLATION_SUCCEEDED",
            PaymentStatus.TransactionCancellationFailed => "TRANSACTION_CANCELLATION_FAILED",
            PaymentStatus.SettlementSucceeded => "SETTLEMENT_SUCCEEDED",
            PaymentStatus.SettlementFailed => "SETTLEMENT_FAILED",
            _ => string.Empty
        };
    }
}