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
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
    IAggregateRepository<CompanyDeposit> companyDepositRepository, IAuthenticationService authenticationService) : IRequestHandler<GetPaymentMethodsCommand, Result<PaymentMethodsViewModel>>
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
    private readonly IAuthenticationService _authenticationService = authenticationService;
    public async Task<Result<PaymentMethodsViewModel>> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestWithChildsByCode(request.ViewModel.PaymentCode));

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
        var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);


        company = await _companyRepository.GetBySpecAsync(new CompanyPaymentMethodsByIdSpec(paymentRequest.CompanyId), cancellationToken);

        if (string.IsNullOrEmpty(sub))
        {
            availablePaymentMethodTypes = new List<PaymentMethodType> { PaymentMethodType.InternetPaymentGateway };
        }
        else
        {
            availablePaymentMethodTypes = company?.PaymentMethods?.Select(p => p.MethodType).ToList();
        }

        if (availablePaymentMethodTypes?.Contains(PaymentMethodType.InternetPaymentGateway) is true)
        {


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



        if (availablePaymentMethodTypes?.Contains(PaymentMethodType.CharismaCard) is true && AvailableCharismaCard(company, paymentRequest))
        {
            var userDepositBalance = await _neoBankService.GetUserDepositBalance();

            if (userDepositBalance?.Data is not null)
            {
                charismaCard = new ViewModels.CharismaCard
                {
                    BalanceAmount = userDepositBalance.Data.Balance,
                    CardNumber = userDepositBalance.Data.CardNumber,
                    CustomerSurname = $"{userDepositBalance.Data.CustomerFirstName} {userDepositBalance.Data.CustomerLastName}",
                    DepositStatus = userDepositBalance.Data.DepositStatus,
                    ExpirationDate = userDepositBalance.Data.ExpirationDate
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




    private bool AvailablePaymentReceipt(Company company, PaymentRequest paymentRequest)
    {
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                                           .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.CharismaCard);

        if (paymentRequestMethod == null)
        {
            return false;
        }

        var paymentRequestMethodDeposits = paymentRequestMethod.PaymentRequestMethodDeposits;

        if (company.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.PaymentReceipt))
        {
            return false;
        }

        if (!company.CompanyDeposits.Any())
        {
            return false;
        }

        var activeMethodDeposits = company.CompanyDeposits.
            Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.PaymentReceipt)).ToList();

        if (!activeMethodDeposits.Any())
        {
            return false;
        }
        if (paymentRequestMethodDeposits.Any())
        {
            var result = activeMethodDeposits.Where(a => paymentRequestMethodDeposits.Select(x => x.Id).Contains(a.Id));
        }

        return true;
    }

    private bool AvailableCharismaCard(Company company, PaymentRequest paymentRequest)
    {
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                                           .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.CharismaCard);

        if (paymentRequestMethod == null)
        {
            return false;
        }
        if (company.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.CharismaCard))
        {
            return false;
        }
        if (!company.CompanyDeposits.Any())
        {
            return false;
        }
        var activeMethodDeposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.CharismaCard)).ToList();

        if (!activeMethodDeposits.Any())
        {
            return false;
        }

        var isDefault = activeMethodDeposits.FirstOrDefault(c => c.IsDefaultForCharismaCard.Value && c.IsActive);

        if (isDefault.Bank.IsActive && isDefault.Bank.IbanPrefix != MiddleEastIbanPrefix)
        {
            return false;
        }
        return true;
    }

    private List<CompanyIPG> AvailableIpg(Company company, PaymentRequest paymentRequest)
    {
        List<CompanyIPG> result = new List<CompanyIPG>();

        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                                            .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.InternetPaymentGateway);
        if (paymentRequestMethod == null)
        {
            result = null;
        }

        var paymentRequestMethodIpgTypes = paymentRequestMethod.PaymentRequestMethodIpgTypes;
        var paymentRequestMethodDeposits = paymentRequestMethod.PaymentRequestMethodDeposits;

        if (!company.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.InternetPaymentGateway))
        {
            return null;
        }

        if (company.CompanyDeposits?.Any() is false)
        {
            return null;
        }

        var activeMethodDeposits = company.CompanyDeposits.
            Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.InternetPaymentGateway)).ToList();

        if (!activeMethodDeposits.Any())
        {
            return null;
        }

        var companyIPGs = company.CompanyIPGs.Where(t => t.Provider.PaymentMethods != null &&
                                                    t.Provider.PaymentMethods.Select(x => x.MethodType)
                                                    .Contains(PaymentMethodType.InternetPaymentGateway) &&
                                                    t.IPGDeposits.Any(c => activeMethodDeposits.Select(a => a.Id)
                                                    .Contains(c.CompanyDepositId)));


        var companyIpgDeposits = companyIPGs.SelectMany(i => i.IPGDeposits);

        if (!companyIPGs.Any())
        { return null; }

        if (!paymentRequestMethodIpgTypes.Any() && !paymentRequestMethodDeposits.Any())//first 
        {

            var companyDefaultIpgDeposits = companyIPGs.SelectMany(t => t.IPGDeposits.Where(t => t.IsDefault));
            if (!companyDefaultIpgDeposits.Any())
                return null;

            result = companyIPGs.Where(i => companyDefaultIpgDeposits.Select(d => d.CompanyIPGId).Contains(i.Id)).ToList();
        }
        else if (!paymentRequestMethodIpgTypes.Any() && paymentRequestMethodDeposits.Any())//second
        {
            var SuggestCompanyDeposits = paymentRequestMethodDeposits.Select(c => c.CompanyDepositId).ToList();

            var contains = companyIpgDeposits.Where(cid => SuggestCompanyDeposits.Contains(cid.Id));

            if (contains.Any())
            {
                return null;
            }
            result = companyIPGs.Where(i => contains.Select(c => c.CompanyIPGId).Contains(i.Id)).ToList();

        }
        else if (paymentRequestMethodIpgTypes.Any() && !paymentRequestMethodDeposits.Any())//third
        {
            var suggestIpgTypes = paymentRequestMethodIpgTypes.Select(c => c.IpgTypeId).ToList();
            var companyIpg = suggestIpgTypes.Where(x => company.CompanyIPGs.Select(c => c.IPGTypeId).Contains(x));
            if (companyIpg.Any())
            {
                return null;
            }
            result = companyIPGs.Where(i => companyIpg.Select(d => d).Contains(i.Id)).ToList();

        }
        else if (paymentRequestMethodIpgTypes.Any() && paymentRequestMethodDeposits.Any())//fourth
        {
            var SuggestCompanyDeposits = paymentRequestMethodDeposits.Select(c => c.CompanyDepositId).ToList();

            var acceptComponyIpgDeposit = companyIpgDeposits.Where(cid => SuggestCompanyDeposits.Contains(cid.Id));


            var suggestIpgTypes = paymentRequestMethodIpgTypes.Select(c => c.IpgTypeId).ToList();
            var acceptCompanyIpg = suggestIpgTypes.Where(x => company.CompanyIPGs.Select(c => c.IPGTypeId).Contains(x));
            if (!acceptCompanyIpg.Any() && !acceptComponyIpgDeposit.Any())
            {
                return null;
            }
            result = companyIPGs.Where(i => acceptCompanyIpg.Select(d => d).Contains(i.Id) &&
                                                 acceptComponyIpgDeposit.Select(d => d.CompanyIPGId).Contains(i.Id)).ToList();

        }



        return result;

    }

    private bool AvailableDirectDebit(Company company, PaymentRequest paymentRequest)
    {
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
                                           .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.DirectDebit);

        if (paymentRequestMethod == null)
        {
            return false;
        }

        var paymentRequestMethodDeposits = paymentRequestMethod.PaymentRequestMethodDeposits;

        if (company.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.DirectDebit))
        {
            return false;
        }
        if (!company.CompanyDeposits.Any())
        {
            return false;
        }
        var activeMethodDeposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.DirectDebit)).ToList();

        if (!activeMethodDeposits.Any())
        {
            return false;
        }
        if (paymentRequestMethodDeposits.Any())
        {
            var result = activeMethodDeposits.Where(a => paymentRequestMethodDeposits.Select(x => x.Id).Contains(a.Id));
        }
        else
        {
            var isDefault = activeMethodDeposits.Any(c => c.IsDefaultForDirectDebit.Value && c.IsActive);

            if (!isDefault)
            {
                return false;
            }
        }
        return true;
    }


}
