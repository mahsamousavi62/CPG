using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    IAggregateRepository<Transaction> transactionRepository,
    ICurrentUser user,
    IMinioProvider minioProvider) : IRequestHandler<GetPaymentMethodsCommand, Result<PaymentMethodsViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _grantRepository = grantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ICurrentUser _user = user;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<PaymentMethodsViewModel>> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByCode(request.ViewModel.PaymentCode));

            if (paymentRequest is null)
            {
                throw new PaymentRequestCodeNotFoundException();
            }

            if (paymentRequest.UrlExpirationDateTime < DateTime.Now)
            {
                throw new PaymentRequestCodeIsExpiredException();
            }

            if (paymentRequest.IsUsed)
            {
                throw new PaymentRequestCodeIsUsedException();
            }

            paymentRequest.Status = Enums.PaymentStatus.RedirectedToCpg;
            await _paymentRequestRepository.UpdateAsync(paymentRequest);

            List<long> availablePaymentMethodTypes = new List<long>();
            Company company = null;
            if (!string.IsNullOrEmpty(paymentRequest.DestinationDepositIban))
            {
                company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIbanSpec(paymentRequest.CompanyId,
                                paymentRequest.DestinationDepositIban), cancellationToken);

                var toBeRemoved = new List<CompanyIPG>();
                foreach (var companyIPGItem in company?.CompanyIPGs)
                {
                    var found = companyIPGItem.IPGDeposits.Any(t => t.CompanyDeposit.Iban == paymentRequest.DestinationDepositIban);
                    if (!found)
                    {
                        toBeRemoved.Add(companyIPGItem);
                    }
                }
                foreach (var companyIPGItem in toBeRemoved)
                {
                    company.CompanyIPGs.Remove(companyIPGItem);
                }
                availablePaymentMethodTypes = company.PaymentMethods.Select(p => p.Id).ToList();
            }
            else
            {
                company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIdSpec(paymentRequest.CompanyId), cancellationToken);
                availablePaymentMethodTypes = company.PaymentMethods.Select(p => p.Id).ToList();

                var toBeRemoved = new List<CompanyIPG>();
                foreach (var companyIPGItem in company?.CompanyIPGs)
                {
                    var defaultDeposit = companyIPGItem.IPGDeposits.FirstOrDefault(t => t.IsDefault);
                    if (defaultDeposit == null) throw new Exception("company not found");
                    if (!defaultDeposit.IsActive)
                    {
                        toBeRemoved.Add(companyIPGItem);
                    }
                }
                foreach (var companyIPGItem in toBeRemoved)
                {
                    company.CompanyIPGs.Remove(companyIPGItem);
                }
            }

            IPGInfo[] ipgResult = null;
            if (company != null && company.IsActive)
            {
                ipgResult = await Task.WhenAll(company.CompanyIPGs?.Select(t => new { t.IPGType, t.Id }).Select(async t => new IPGInfo
                {
                    Id = t.Id,
                    Logo = await _minioProvider.PresignedGetObject(t.IPGType.Logo),
                    PersianName = t.IPGType.PersianName,
                })).ConfigureAwait(false);
            }

            var userGrants = await _grantRepository.ListAsync(new DirectDebitGrantByUserSpec(_user.UserId, paymentRequest.Amount, company.NationalCodeMatchingRequied), cancellationToken);

            foreach (var item in userGrants)
            {
                if (item.Bank?.DirectDebitSetting.ProviderId != item.ProviderId)
                {
                    userGrants.Remove(item);
                }
                else
                {
                    var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(item.Id), cancellationToken);
                    var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(item.Id), cancellationToken);

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
                            var currentDayTransactions = await _transactionRepository.ListAsync(new CurrentDayTransactionByGrantIdSpec(item.Id), cancellationToken);

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
                                var currentMonthTransactions = await _transactionRepository.ListAsync(new CurrentMonthTransactionByGrantIdSpec(item.Id), cancellationToken);

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
                    Logo = await _minioProvider.PresignedGetObject(t.FirstOrDefault().Bank.LogoAddress)
                },
                GrantInfo = t.Select(q => new DirectDebitGrantInfo
                {
                    AccountNumber = q.AccountNumber,
                    Id = q.Id,
                }).ToList(),
            })).ConfigureAwait(false);

            return Result<PaymentMethodsViewModel>.SuccessResult(new PaymentMethodsViewModel
            {
                Amount = paymentRequest.Amount,
                AvailablePaymentMethodTypes=availablePaymentMethodTypes,
                PaymentCode=paymentRequest.PaymentCode,
                IPGs = ipgResult?.ToList(),
                DirectDebits = directDebits?.ToList(),
                CompanyName = company.PersianName,
            });
        }
        catch (DomainException exc)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<PaymentMethodsViewModel>.Failure(new Error("1006000", GlobalResource.PaymentMethodsUnexpectedError));
        }
    }
}
