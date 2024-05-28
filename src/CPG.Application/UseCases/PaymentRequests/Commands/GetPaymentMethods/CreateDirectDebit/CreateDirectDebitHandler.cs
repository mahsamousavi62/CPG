using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using Ardalis.Specification;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateDirectDebit;

public class CreateDirectDebitHandler : GetPaymentMethodsHandler
{
    private bool AvailableDirectDebit(RequestContext request)
    {
        var validator = new CreateDirectDebitValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return false;
        }

        var company = request.Company;
        var paymentRequest = request.PaymentRequest;
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                                        .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.DirectDebit);
        var paymentRequestMethodDeposits = paymentRequestMethod.PaymentRequestMethodDeposits;
        var hasDeposits = paymentRequestMethodDeposits.Any();

        var directDebitCompanyDeposits = company.CompanyDeposits
            .Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.DirectDebit))
            .ToList();

        if (hasDeposits)
        {
            return directDebitCompanyDeposits
                .Any(a => paymentRequestMethodDeposits.Select(x => x.Id).Contains(a.Id));
        }
        else
        {
            var defaultDirectDebitDeposits = directDebitCompanyDeposits
                .Any(c => c.IsDefaultForDirectDebit.HasValue && c.IsDefaultForDirectDebit.Value && c.IsActive);

            return defaultDirectDebitDeposits ? true : false;
        }
    }

    public override async Task HandleRequset(PaymentMethodType methodType, RequestContext request, PaymentMethodsViewModel model)
    {
        var company = request.Company;
        var _user = request.CurrentUser;
        var _minioProvider = request.MinioProvider;
        var _grantRepository = request.GrantRepository;
        var _transactionRepository = request.TransactionRepository;

        if (methodType == PaymentMethodType.DirectDebit && AvailableDirectDebit(request))
        {
            var paymentRequest = request.PaymentRequest;
            
            var userGrants = await _grantRepository.ListAsync(new DirectDebitGrantByUserSpec(_user.UserId, paymentRequest.Amount, company.NationalCodeMatchingRequied));

            foreach (var item in userGrants)
            {
                if (item.Bank?.DirectDebitSetting.ProviderId != item.ProviderId)
                {
                    userGrants.Remove(item);
                }
                else
                {
                    var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(item.Id));
                    var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(item.Id));

                    if (paymentRequest.Amount > item.Bank.DirectDebitSetting.MaxWithdrawalAmountPerDay - currentDayTransactions.Sum(t => t.Amount))
                    {
                        userGrants.Remove(item);
                    }
                    else if (item.SuccessTransactionCountLimitPerMonth - currentMonthTransactions.Count() <= 0)
                    {
                        userGrants.Remove(item);
                    }
                }
            }

            var groupedGrants = userGrants.GroupBy(t => t.AccountNumber).ToList();
            
            var grants = new List<DirectDebitGrant>();
            
            foreach (var group in groupedGrants)
            {
                var selectedItems = group.ToList();
                if (selectedItems.Count() > 1)
                {
                    var maxAmountLimit = selectedItems.Max(t => t.AmountLimitPerTransaction);
                    selectedItems = selectedItems.Where(t => t.AmountLimitPerTransaction == maxAmountLimit).ToList();
                    if (selectedItems.Count() > 1)
                    {
                        var amountlist = new Dictionary<long, decimal>();
                        foreach (var item in selectedItems)
                        {
                            var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(item.Id));

                            var remainedAmount = item.Bank.DirectDebitSetting.MaxWithdrawalAmountPerDay - currentDayTransactions.Sum(t => t.Amount);
                            amountlist.Add(item.Id, remainedAmount);
                        }
                        var minRemainedAmount = amountlist.Min(t => t.Value);
                        selectedItems = selectedItems.Where(t => amountlist.Where(l => l.Value == minRemainedAmount).Select(l => l.Key).Contains(t.Id)).ToList();

                        if (selectedItems.Count() > 1)
                        {
                            var countlist = new Dictionary<long, decimal>();
                            foreach (var item in selectedItems)
                            {
                                var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(item.Id));

                                var remainedCount = item.SuccessTransactionCountLimitPerMonth - currentMonthTransactions.Count();
                                countlist.Add(item.Id, remainedCount);
                            }
                            var minRemainedCount = countlist.Min(t => t.Value);
                            selectedItems = selectedItems.Where(t => countlist.Where(l => l.Value == minRemainedCount).Select(l => l.Key).Contains(t.Id)).ToList();
                        }
                    }
                }
                grants.Add(selectedItems.OrderBy(t => t.ExpirationDate).FirstOrDefault());
            }
            
            var directDebits = await Task.WhenAll(grants?.GroupBy(t => t.BankId).Select(async t => new DirectDebitInfo
            {
                BankInfo = new DirectDebit.ViewModels.AvailableBankViewModel
                {
                    Id = t.Key,
                    Name = t.FirstOrDefault().Bank.Name,
                    Logo = await _minioProvider.PresignedGetObject(t.FirstOrDefault().Bank.Logo)
                },
                GrantInfo = t.Select(q => new DirectDebitGrantInfo
                {
                    AccountNumber = q.AccountNumber,
                    Id = q.Id,
                }).ToList(),
            })).ConfigureAwait(false);
            
            model.DirectDebits = directDebits.ToList();
        }
        else if (handler != null)
        {
            await handler.HandleRequset(methodType, request, model);
        }
    }
}
