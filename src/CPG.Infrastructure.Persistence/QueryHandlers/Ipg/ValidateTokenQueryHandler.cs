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
                            transaction.IPGTransaction.PredicateExpirationDateTime = DateTime.UtcNow.AddMinutes(transaction.IPGTransaction.VerificationTimeLimit);
                            paymentRequest.Status = PaymentStatus.TransactionWaitingForVerification;
                        }
                        break;
                    }
                case ProviderType.Pec:
                    {
                        var req = System.Text.Json.JsonSerializer.Deserialize<PecValidateTokenViewModel>(request.ValidateToken.Request);

                        transaction.IPGTransaction.ProviderTrackerId = req.STraceNo.ToString();
                        transaction.IPGTransaction.ReferenceNumber = req.RRN.ToString();
                        transaction.IPGTransaction.EncryptCardNumber = req.HashCardNumber;

                        if (req.Status != 0)
                        {
                            transaction.IPGTransaction.Status = IPGTransactionStatus.Failed;
                            transaction.Status = TransactionStatus.TransactionFailed;
                            paymentRequest.Status = PaymentStatus.TransactionFailed;
                        }
                        else if (req.Status == 0)
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
                CallbackUrl = $"{paymentRequest.CallBackUrl}/paymentResult?paymentCode={paymentRequest.PaymentCode}&paymentStatus={General.GetPaymentStatusTitle(paymentRequest.Status)}"
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


}