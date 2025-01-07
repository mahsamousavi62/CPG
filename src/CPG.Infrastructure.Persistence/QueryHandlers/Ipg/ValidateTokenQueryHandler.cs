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
using System.Linq;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class ValidateTokenQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Transaction> transactionRepository) : IRequestHandler<ValidateTokenQuery, Result<ValidateTokenResponseViewModel>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly string[] sepUnsuccessStatusList = ["1", "3", "4", "5", "8", "10", "11", "12", "21"];
    private readonly string[] sepSuccessStatusList = ["2"];
    private readonly string PecSucceedStatus = "0";
    private readonly string bepSucceedStatus = "0";
    private readonly string ayandehSucceedStatus = "0";

    public async Task<Result<ValidateTokenResponseViewModel>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByIPGTrackId(request.TrackId));
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
                                    transaction.PredictedSettlementDateTime = GetPredictedSettlementTimeDueToCurrentTime();
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
                        var req = System.Text.Json.JsonSerializer.Deserialize<SepValidateTokenViewModel>(request.ValidateToken.Request);

                        transaction.IPGTransaction.ProviderTrackerId = req.RefNum;
                        transaction.IPGTransaction.ReferenceNumber = req.Rrn;
                        transaction.IPGTransaction.EncryptCardNumber = req.HashedCardNumber;

                        if (sepUnsuccessStatusList.Contains(req.Status))
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
                        }
                        else if (sepSuccessStatusList.Contains(req.Status))
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.Now.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                            transaction.PredictedSettlementDateTime = GetPredictedSettlementTimeDueToCurrentTime();
                        }
                        break;
                    }
                case ProviderType.Pec:
                    {
                        var req = System.Text.Json.JsonSerializer.Deserialize<PecValidateTokenViewModel>(request.ValidateToken.Request);

                        transaction.IPGTransaction.ProviderTrackerId = req.STraceNo;
                        transaction.IPGTransaction.ReferenceNumber = req.RRN;
                        transaction.IPGTransaction.EncryptCardNumber = req.HashCardNumber;

                        if (req.Status != PecSucceedStatus)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
                        }
                        else if (req.Status == PecSucceedStatus)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.Now.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                            transaction.PredictedSettlementDateTime = GetPredictedSettlementTimeDueToCurrentTime();
                        }
                        break;
                    }
                case ProviderType.BehPardakht:
                    {
                        var req = System.Text.Json.JsonSerializer.Deserialize<BehPardakhtValidateTokenViewModel>(request.ValidateToken.Request);

                        transaction.IPGTransaction.ProviderTrackerId = req.SaleReferenceId;
                        transaction.IPGTransaction.ReferenceNumber = req.SaleReferenceId;
                        transaction.IPGTransaction.EncryptCardNumber = req.CardHolderInfo;

                        if(req.ResCode == bepSucceedStatus)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.Now.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                            transaction.PredictedSettlementDateTime = GetPredictedSettlementTimeDueToCurrentTime();
                        }
                        else
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
                        }

                        break;
                    }
                case ProviderType.Ayandeh:
                    {
                        var req = System.Text.Json.JsonSerializer.Deserialize<AyandehValidateTokenViewModel>(request.ValidateToken.Request);

                        transaction.IPGTransaction.ProviderTrackerId = req.TraceNumber;
                        transaction.IPGTransaction.ReferenceNumber = req.ChannelRefNumber;

                        if (req.Result == ayandehSucceedStatus)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.SucceededAndWaitingForVerification;
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.Now.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                            transaction.PredictedSettlementDateTime = GetPredictedSettlementTime();
                        }
                        else
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
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
                CallbackUrl = Constants.CreateCallbackUrl(paymentRequest.CallBackUrl, paymentRequest.PaymentCode, paymentRequest.Status)
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

    private static DateTime GetPredictedSettlementTimeDueToCurrentTime()
    {
        var currentDateTime = DateTime.Now;
        var timeMargin = new TimeOnly(23, 45);
        var currentTime = new TimeOnly(currentDateTime.Hour, currentDateTime.Minute);
        var date = currentTime < timeMargin ?
            new DateTime(currentDateTime.AddDays(1).Year, currentDateTime.AddDays(1).Month, currentDateTime.AddDays(1).Day, 7, 0, 0) :
            new DateTime(currentDateTime.AddDays(2).Year, currentDateTime.AddDays(2).Month, currentDateTime.AddDays(2).Day, 7, 0, 0);
        return date;
    }

    private static DateTime GetPredictedSettlementTime()
    {
        var currentDateTime = DateTime.Now;
        return new DateTime(currentDateTime.AddDays(2).Year, currentDateTime.AddDays(2).Month, currentDateTime.AddDays(2).Day, 7, 0, 0);
    }
}