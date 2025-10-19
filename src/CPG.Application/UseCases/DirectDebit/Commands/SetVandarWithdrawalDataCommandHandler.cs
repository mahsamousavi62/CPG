using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using System.Text.Json;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Helper;
using Microsoft.AspNetCore.SignalR;
using CPG.Application.Shared.Resource;
using System.Collections.Generic;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Application.Shared.Interfaces;
using CPG.Application.Shared;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetVandarWithdrawalDataCommandHandler(
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogService logService,
    IHubContext<NotificationHub, INotificationHub> notificationHub
    ) : IRequestHandler<SetVandarWithdrawalDataCommand>
{
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = grantRepository;
    private readonly ILogService _logService = logService;
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub = notificationHub;
    private readonly List<string> failedStatusArray = ["FAILED", "CANCELED", "REVERSED"];
    private readonly string SuccessStatus = "DONE";

    public async Task Handle(SetVandarWithdrawalDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grant = await _directDebitGrantRepository.GetBySpecAsync(new DirectDebitGrantByAuthorizationIdSpec(request.model.AuthorizationId), cancellationToken);
            if (grant is null)
            {
                throw new DirectDebitGrantNotFoundException();
            }

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByProviderTrackerIdSpec(request.model.WithdrawalId), cancellationToken);            
            var withdrawData = JsonSerializer.Deserialize<WithdrawData>(transaction.DirectDebitTransaction.ProviderData);
            if (!string.IsNullOrEmpty(request.model.WithdrawalId))
            {
                withdrawData.Id = request.model.WithdrawalId;
            }
            if (!string.IsNullOrEmpty(request.model.AuthorizationId))
            {
                withdrawData.AuthorizationId = request.model.AuthorizationId;
            }
            if (request.model.GatewayTransactionId is not null)
            {
                withdrawData.GatewayTransactionId = (long)request.model.GatewayTransactionId;
            }
            if (!string.IsNullOrEmpty(request.model.Status))
            {
                withdrawData.Status = request.model.Status;
            }
            if (request.model.Amount is not null)
            {
                withdrawData.Amount = request.model.Amount.ToString();
            }
            if (!string.IsNullOrEmpty(request.model.WageAmount))
            {
                withdrawData.WageAmount = request.model.WageAmount;
            }
            if (request.model.PaymentNumber is not null)
            {
                withdrawData.PaymentNumber = (long)request.model.PaymentNumber;
            }
            if (!string.IsNullOrEmpty(request.model.ErrorCode))
            {
                withdrawData.ErrorCode = request.model.ErrorCode;
            }
            if (!string.IsNullOrEmpty(request.model.ErrorMessage))
            {
                withdrawData.ErrorMessage = request.model.ErrorMessage;
            }
            if (request.model.PayerAccount is not null)
            {
                withdrawData.PayerAccount = new WithdrawPayerAccountData
                {
                    AccountNumber = request.model.PayerAccount.AccountNumber,
                    Pan = request.model.PayerAccount.Pan,
                };
            }

            var status = SharedServices.GetDirectDebitTransactionStatus(request.model.Status);
            transaction.DirectDebitTransaction.Status = status;
            transaction.DirectDebitTransaction.ProviderData = JsonSerializer.Serialize(withdrawData);
            transaction.Status = status == Enums.DirectDebitTransactionStatus.UnSuccessful ? Enums.TransactionStatus.TransactionFailed : transaction.Status;
            transaction.PaymentRequest.Status = status == Enums.DirectDebitTransactionStatus.UnSuccessful ? Enums.PaymentStatus.TransactionFailed :
                status == Enums.DirectDebitTransactionStatus.TransactionSucceeded ? Enums.PaymentStatus.TransactionWaitingForVerification : transaction.PaymentRequest.Status;
            transaction.PredictedSettlementDateTime = GetPredictedSettlementTime();
            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(transaction.PaymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();

            if (string.IsNullOrEmpty(grant.AccountNumber) && !string.IsNullOrEmpty(request.model.PayerAccount?.AccountNumber))
            {
                grant.AccountNumber = request.model.PayerAccount.AccountNumber;
                await _directDebitGrantRepository.UpdateAsync(grant);
                await _directDebitGrantRepository.SaveChangesAsync();
            }

            await _notificationHub.Clients.All.SendMessage(GetNotificationData(withdrawData, transaction));
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(SetVandarWithdrawalDataCommandHandler),
                providerName: "DirectDebit",
                requestUri: nameof(SetVandarWithdrawalDataCommand),
                requestBody: JsonSerializer.Serialize(request),
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.VandarStore,
                providerType: Enums.ProviderTypeInLog.Vandar,
                auditType: Enums.AuditType.Client,
                userId: 1
            );
            _logService.LogError(callLog);
        }
    }

    private Result<WithdrawalDataResponseViewModel> GetNotificationData(WithdrawData withdrawData, Transaction transaction)
    {
        switch (withdrawData.ErrorCode)
        {
            case "00":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010001", GlobalResource.ServerMalfunction));
            case "01":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010002", GlobalResource.NotEnoughBalance));
            case "02":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010003", GlobalResource.ServerMalfunction));
            case "03":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010004", GlobalResource.ServerMalfunction));
            case "04":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010005", GlobalResource.DailyTransactionLimit));
            case "05":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010006", GlobalResource.MonthlyTransactionNumber));
            case "06":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010007", GlobalResource.MonthlyTransactionNumber));
            case "07":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010008", GlobalResource.InvalidTransactionTime));
            case "08":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010009", GlobalResource.IllegalTransactionAmount));
            case "09":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010010", GlobalResource.DailyTransactionAmount));
            case "10":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010011", GlobalResource.BankMalfunction));
            case "11":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010012", GlobalResource.BankAmountLimit));
            case "12":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010013", GlobalResource.DiffrentNumber));
            case "13":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010014", GlobalResource.InvalidGrant));
            case "14":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010015", GlobalResource.ExpiredGrant));
            case "15":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010016", GlobalResource.InvalidTransactionTime));
            case "16":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010017", GlobalResource.DepositProblem));
            case "17":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010018", GlobalResource.InactiveGrant));
            case "18":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010019", GlobalResource.InactiveCard));
            case "19":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010020", GlobalResource.ExpiredCard));
            case "20":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010021", GlobalResource.InvalidCardData));
            case "21":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010022", GlobalResource.InvalidDeposit));
            case "22":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010023", GlobalResource.InvalidDepositNumber));
            case "23":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010024", GlobalResource.ServerMalfunction));
            case "24":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010025", GlobalResource.InvalidCardOrDeposit));
            case "26":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010026", GlobalResource.NoShahabCode));
            case "28":
                return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010027", GlobalResource.BlockedAccount));
            case null or "":
                {
                    if (withdrawData.Status == SuccessStatus)
                    {
                        return Result<WithdrawalDataResponseViewModel>.SuccessResult(new WithdrawalDataResponseViewModel
                        {
                            CallbackUrl = Constants.CreateCallbackUrl(transaction.PaymentRequest.CallBackUrl, transaction.PaymentRequest.PaymentCode, transaction.PaymentRequest.Status)
                        });
                    }
                    else if (failedStatusArray.Contains(withdrawData.Status))
                    {
                        return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010028", GlobalResource.UseOtherBanks));
                    }
                    return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010000", GlobalResource.UnexpectedError));
                }
            default:
                {
                    if (failedStatusArray.Contains(withdrawData.Status))
                    {
                        return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010028", GlobalResource.UseOtherBanks));
                    }
                    return Result<WithdrawalDataResponseViewModel>.Failure(new Error("2010000", GlobalResource.UnexpectedError));
                }
        }
    }

    private static DateTime GetPredictedSettlementTime()
    {
        var currentDateTime = DateTime.Now;
        return new DateTime(currentDateTime.AddDays(1).Year, currentDateTime.AddDays(1).Month, currentDateTime.AddDays(1).Day, 0, 0, 0);
    }
}