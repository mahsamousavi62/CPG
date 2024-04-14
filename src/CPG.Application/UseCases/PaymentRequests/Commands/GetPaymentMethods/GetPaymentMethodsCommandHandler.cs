using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    IAggregateRepository<Transaction> transactionRepository,
    ICurrentUser user, IMinioProvider minioProvider, INeoBankService neoBankService,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository) : IRequestHandler<GetPaymentMethodsCommand, Result<PaymentMethodsViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _grantRepository = grantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly ICurrentUser _user = user;
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly INeoBankService _neoBankService = neoBankService;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly string MiddleEastIbanPrefix = "078";

    public async Task<Result<PaymentMethodsViewModel>> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
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

        Company company = null;
        List<PaymentMethodType> availablePaymentMethodTypes = null;
        Receipt receipt = null;
        ViewModels.CharismaCard charismaCard = null;

        if (!string.IsNullOrEmpty(paymentRequest.DestinationDepositIban))
        {
            company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIbanSpec(paymentRequest.CompanyId,
                            paymentRequest.DestinationDepositIban), cancellationToken);
            availablePaymentMethodTypes = company?.PaymentMethods?.Select(p => p.MethodType).ToList();

            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.InternetPaymentGateway) is true)
            {
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
            }
            var companyDeposit = company.CompanyDeposits?.Where(t => t.Iban == paymentRequest.DestinationDepositIban).FirstOrDefault();
            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.PaymentReceipt) is true)
            {
                if (companyDeposit is not null)
                {
                    receipt = new Receipt
                    {
                        AccountNumber = companyDeposit?.AccountNumber,
                        BankName = companyDeposit?.Bank?.Name,
                        DestinationDepositId = companyDeposit?.Id
                    };
                }
            }

            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.CharismaCard) is true &&
                 companyDeposit.Bank.IbanPrefix == MiddleEastIbanPrefix)
            {
                var userDepositBalance = await _neoBankService.GetUserDepositBalance();

                charismaCard = userDepositBalance.Data.DepositStatus switch
                {
                    _ => new ViewModels.CharismaCard
                    {
                        BalanceAmount = userDepositBalance.Data.Balance,
                        CardNumber = userDepositBalance.Data.CardNumber,
                        CustomerSurname = $"{userDepositBalance.Data.CustomerFirstName} {userDepositBalance.Data.CustomerLastName}",
                        DepositStatus = userDepositBalance.Data.DepositStatus,
                        ExpirationDate = userDepositBalance.Data.ExpirationDate
                    }
                };
            }
        }
        else
        {
            company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIdSpec(paymentRequest.CompanyId), cancellationToken);
            availablePaymentMethodTypes = company?.PaymentMethods?.Select(p => p.MethodType).ToList();

            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.InternetPaymentGateway) is true)
            {
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
            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.PaymentReceipt) is true)
            {
                receipt = new Receipt
                {
                    AccountNumber = string.Empty,
                    BankName = string.Empty,
                    DestinationDepositId = null
                };
            }

            var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new DefaultDirectDebitDepositSpec(paymentRequest.CompanyId), cancellationToken);
            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.CharismaCard) is true && companyDeposit != null &&
                companyDeposit.Bank.IbanPrefix == MiddleEastIbanPrefix)
            {
                var userDepositBalance = await _neoBankService.GetUserDepositBalance();

                charismaCard = userDepositBalance.Data.DepositStatus switch
                {
                    _ => new ViewModels.CharismaCard
                    {
                        BalanceAmount = userDepositBalance.Data.Balance,
                        CardNumber = userDepositBalance.Data.CardNumber,
                        CustomerSurname = $"{userDepositBalance.Data.CustomerFirstName} {userDepositBalance.Data.CustomerLastName}",
                        DepositStatus = userDepositBalance.Data.DepositStatus,
                        ExpirationDate = userDepositBalance.Data.ExpirationDate
                    }
                };
            }

        }

        IPGInfo[] ipgResult = null;
        DirectDebitInfo[] directDebits = null;
        if (company != null && company.IsActive)
        {
            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.InternetPaymentGateway) is true)
            {
                ipgResult = await Task.WhenAll(company.CompanyIPGs?.Select(t => new { t.IPGType, t.Id }).Select(async t => new IPGInfo
                {
                    Id = t.Id,
                    Logo = await _minioProvider.PresignedGetObject(t.IPGType.Logo),
                    PersianName = t.IPGType.PersianName,
                })).ConfigureAwait(false);
            }
            if (availablePaymentMethodTypes?.Contains(PaymentMethodType.DirectDebit) is true)
            {
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
                directDebits = await Task.WhenAll(grants?.GroupBy(t => t.BankId).Select(async t => new DirectDebitInfo
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
            }
        }

        return Result<PaymentMethodsViewModel>.SuccessResult(new PaymentMethodsViewModel
        {
            Amount = paymentRequest.Amount,
            PaymentCode = paymentRequest.PaymentCode,
            IPGs = ipgResult?.ToList(),
            DirectDebits = directDebits?.ToList(),
            Receipt = receipt,
            CharismaCard = charismaCard,
            CompanyName = company?.PersianName,
        });
    }
}
