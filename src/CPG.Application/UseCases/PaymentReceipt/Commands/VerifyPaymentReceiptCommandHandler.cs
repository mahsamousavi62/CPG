

using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using CPG.Domain.SharedKernel.Communication.Ipg;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Application.UseCases.PaymentReceipt.Exceptions;
using CPG.Application.UseCases.Companies.Exceptions;
using System.Security.Claims;
using System.Globalization;
using Ardalis.GuardClauses;

namespace CPG.Application.UseCases.PaymentReceipt.Commands;

public class VerifyPaymentReceiptCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
                                                IAggregateRepository<Transaction> transactionRepository,
                                                IHttpContextAccessor httpContext, ICurrentUser currentUser)
                                                : IRequestHandler<VerifiyPaymentReceiptCommand, Result<string>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<string>> Handle(VerifiyPaymentReceiptCommand request, CancellationToken cancellationToken)
    {

        Guard.Against.NegativeOrZero(request.VerifyTransaction.Id, nameof(request.VerifyTransaction));  

        var transaction = await _transactionRepository.GetBySpecAsync
                                (new TransactionByPaymentReceiptId(request.VerifyTransaction.Id), cancellationToken);

        if (transaction is null)
        {
            throw new PaymentReceiptNotFoundException(request.VerifyTransaction.Id);
        }

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
         c.Value == UserRoleType.SuperAdmin.GetValue());
        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                if (transaction.PaymentRequest.CompanyId != companyId)
                {
                    throw new NotAuthorizeToVerifyException();
                }
            }
        }

        if (transaction.PaymentRequest.Status != PaymentStatus.TransactionWaitingForVerification &&
            transaction.PaymentRequest.Status != PaymentStatus.TransactionVerificationFailed &&
            transaction.PaymentRequest.Status != PaymentStatus.TransactionVerifiedByApplication)
        { throw new VerifyPaymentRequestStatusException(); }

        if (transaction.PaymentReceiptTransaction.Status != PaymentReceiptStatus.SucceededAndWaitingForVerification)
        {
            throw new VerifyPaymentReceiptStatusException();
        }

        try
        {
            var paymentRequest = transaction.PaymentRequest;

            paymentRequest.Status = PaymentStatus.TransactionVerifiedByApplication;
            paymentRequest.ModificationDate ??= DateTime.Now;

            await _paymentRequestRepository.UpdateAsync(paymentRequest, cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

            switch (transaction.TransactionMethodType)
            {
                case TransactionType.PaymentReceipt:
                    {
                        transaction.PredictedSettlementDateTime = transaction.PaymentReceiptTransaction.ReceiptDateTime;
                        transaction.Status = TransactionStatus.TransactionSucceeded;
                        paymentRequest.Status = PaymentStatus.TransactionVerificationSucceeded;
                        transaction.PaymentReceiptTransaction.Status = PaymentReceiptStatus.SucceededAndWaitingForVerification;
                        transaction.PaymentReceiptTransaction.ModificationDate = DateTime.Now;
                        transaction.PaymentReceiptTransaction.VerificationDateTime = DateTime.Now;
                        transaction.PaymentReceiptTransaction.VerifiedBy = _currentUser.UserId;

                        break;
                    }
                default:
                    break;
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            await _paymentRequestRepository.UpdateAsync(paymentRequest);
            await _paymentRequestRepository.SaveChangesAsync();

            return Result<string>.SuccessResult(GlobalResource.VerifiedMessage);
        }
        catch (DomainException exc)
        {
            return Result<string>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<string>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<string>.Failure(new Error("1015000", GlobalResource.TransactionDetailUnexpectedError));
        }

    }
}
