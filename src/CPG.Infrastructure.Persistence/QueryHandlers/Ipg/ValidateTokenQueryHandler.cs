using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.TransactionAggregate;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.Shared.Resource;
using CPG.Domain.Exceptions;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class ValidateTokenQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Transaction> transactionRepository) : IRequestHandler<ValidateTokenQuery, Result<ValidateTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;

    public async Task<Result<ValidateTokenResponseViewModel>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByIPGTrackId(request.ValidateToken.TrackId));

            if (transaction?.IPGTransaction is null)
                throw new NotFoundTrackIdException();
            if (transaction.IPGTransaction.Status != IPGTransactionStatus.WaitingForPspResponse)
                throw new TrackIdInvalidStatusException();

            var paymentRequest = await _paymentRequestRepository.GetByIdAsync(transaction.PaymentRquestId);
            var providerType = transaction.IPGTransaction.CompanyIPG.Provider.ProviderType;
            switch (providerType)
            {
                case ProviderType.Vandar:
                    break;
                case ProviderType.AsanPardakht:
                    {
                        var ipg = _ipgFactory.GetInstance(providerType);
                        var result = await ipg.GetTransactionResult(new TransactionResultRequest
                        {
                            ProviderData = transaction.IPGTransaction.CompanyIPG.ProviderData,
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
                                    transaction.IPGTransaction.PredicateExpirationDateTime = result.PayGateTranDate.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
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

                        break;
                    }
                case ProviderType.Sep:
                    {
                        transaction.IPGTransaction.ProviderTrackerId = request.ValidateToken.RefNum;
                        transaction.IPGTransaction.ReferenceNumber = request.ValidateToken.Rrn;
                        transaction.IPGTransaction.EncryptCardNumber = request.ValidateToken.HashedCardNumber;

                        if (request.ValidateToken.Status is 1 or 3 or 4 or 5 or 8 or 10 or 11 or 12 or 21)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
                        }
                        else if(request.ValidateToken.Status is 2)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.UtcNow.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                        }
                        break;
                    }
                default:
                    break;
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _transactionRepository.SaveChangesAsync();

            return Result<ValidateTokenResponseViewModel>.SuccessResult(new ValidateTokenResponseViewModel
            {
                CallbackUrl = $"{paymentRequest.CallBackUrl}/paymentResult?paymentCode={paymentRequest.PaymentCode}&paymentStatus={GetStatusTitle(paymentRequest.Status)}"
            });
        }
        catch (DomainException exc)
        {
            return Result<ValidateTokenResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<ValidateTokenResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<ValidateTokenResponseViewModel>.Failure(new Error("1008000", GlobalResource.GetPaymentTicketUnexpectedError));
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