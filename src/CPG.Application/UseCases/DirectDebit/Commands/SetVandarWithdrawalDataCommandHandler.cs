using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using System.Text.Json;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Helper;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetVandarWithdrawalDataCommandHandler(
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogger<GetDirectDebitPlansCommandHandler> logger    
    ) : IRequestHandler<SetVandarWithdrawalDataCommand, Result<bool>>
{
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = grantRepository;
    private readonly ILogger<GetDirectDebitPlansCommandHandler> _logger = logger;
    
    public async Task<Result<bool>> Handle(SetVandarWithdrawalDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grant = await _directDebitGrantRepository.GetBySpecAsync(new DirectDebitGrantByAuthorizationIdSpec(request.model.AuthorizationId), cancellationToken);
            if (grant is null)
            {
                throw new DirectDebitGrantNotFoundException();
            }

            var transaction = await _transactionRepository.GetBySpecAsync(new TransactionByDDGrantIdSpec(grant.Id), cancellationToken);
            var status = SharedServices.GetDirectDebitTransactionStatus(request.model.Status);
            transaction.DirectDebitTransaction.Status = status;
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
            if (!string.IsNullOrEmpty(request.model.Amount))
            {
                withdrawData.Amount = request.model.Amount;
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

            transaction.DirectDebitTransaction.ProviderData = JsonSerializer.Serialize(withdrawData);
            transaction.Status = status == Enums.DirectDebitTransactionStatus.UnSuccessful ? Enums.TransactionStatus.TransactionFailed : transaction.Status;
            transaction.PaymentRequest.Status = status == Enums.DirectDebitTransactionStatus.UnSuccessful ? Enums.PaymentStatus.TransactionFailed :
                status == Enums.DirectDebitTransactionStatus.TransactionSucceeded ? Enums.PaymentStatus.TransactionVerificationSucceeded : transaction.PaymentRequest.Status;
            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            if (string.IsNullOrEmpty(grant.AccountNumber) && !string.IsNullOrEmpty(request.model.PayerAccount?.AccountNumber))
            {
                grant.AccountNumber = request.model.PayerAccount.AccountNumber;
            }
            await _directDebitGrantRepository.UpdateAsync(grant);
            await _directDebitGrantRepository.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<bool>.FailureResult(false, new Error(exc.Source, exc.Message));
        }
    }
}