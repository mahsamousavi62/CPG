using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Application.UseCases.Banks.ViewModels;
using System.Linq;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using System.Numerics;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.SharedKernel.Helper;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetDirectDebitPlansCommandHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<DirectDebitPlan> planRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogger<GetDirectDebitPlansCommandHandler> logger,
    IAggregateRepository<Provider> providerRepository,
    IAuthenticationService authenticationService,
    ICurrentUser user
    ) : IRequestHandler<GetDirectDebitPlansCommand, Result<PlanViewModel>>
{
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<DirectDebitPlan> _directDebitPlanRepository = planRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = grantRepository;
    private readonly ILogger<GetDirectDebitPlansCommandHandler> _logger = logger;
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ICurrentUser _user = user;

    public async Task<Result<PlanViewModel>> Handle(GetDirectDebitPlansCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetBySpecAsync(new BankByIdSpec(request.model.BankId), cancellationToken);
            if (bank == null)
            {
                throw new BankNotFoundException(request.model.BankId);
            }

            var directDebitProvider = _directDebitFactory.GetInstance(bank.DirectDebitSetting.Provider.ProviderType);
            var provider = bank.DirectDebitSetting.Provider;
            var tokenResult = await directDebitProvider.GetTokenAsync(new TokenRequest { ProviderData = provider.ProviderData });

            var providerData = JObject.Parse(provider.ProviderData);
            if (providerData["Refresh_Token"].ToString() != tokenResult.RefreshToken)
            {
                providerData["Refresh_Token"] = tokenResult.RefreshToken;
                provider.ProviderData = Newtonsoft.Json.JsonConvert.SerializeObject(providerData);
                await _providerRepository.UpdateAsync(provider);
                await _providerRepository.SaveChangesAsync();
            }

            var grants = await _directDebitGrantRepository.ListAsync(new DirectDebitGrantByUserAndBankSpec(bank.Id, _user.UserId, bank.DirectDebitSetting.ProviderId), cancellationToken);
            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);

            var phoneNumbers = grants.Where(t => t.PhoneNumber != mobileNumber).Select(t => t.PhoneNumber).Distinct().ToList();
            if (phoneNumbers?.Any(t => t == mobileNumber) is false)
                phoneNumbers?.Add(mobileNumber);

            foreach (var number in phoneNumbers)
            {
                var grantData = await directDebitProvider.GetUserGrants(new UserGrantsRequest
                {
                    AccessToken = tokenResult.AccessToken,
                    MobileNumber = number,
                    ProviderData = provider.ProviderData,
                });

                foreach (var grant in grantData.Grants)
                {
                    var status = SharedServices.GetVandarDirectDebitGrantStatus(grant.Status);                    
                    var dbGrant = await _directDebitGrantRepository.GetBySpecAsync(new DirectDebitGrantByAuthorizationIdSpec(grant.Id), cancellationToken);
                    var expirationDate = DateTime.Parse(grant.ExpirationDate);
                    if (dbGrant is null)
                    {
                        var durationPerMonth = (expirationDate.Year - DateTime.Now.Year) * 12 + expirationDate.Month - DateTime.Now.Month;
                        var newGrant = DirectDebitGrant.Create(_user.UserId, bank.Id, grant.AccountNumber, grant.Mobile, grant.Count, grant.Limit,
                            null, DateTime.Parse(grant.ExpirationDate), DateTime.Parse(grant.RevokedAt), provider.Id, grant.Token, grant.Id, status, (short)durationPerMonth);

                        await _directDebitGrantRepository.AddAsync(newGrant);
                        await _directDebitGrantRepository.SaveChangesAsync();
                    }
                    else
                    {
                        DirectDebitGrant.Update(dbGrant, _user.UserId, bank.Id, grant.AccountNumber, grant.Mobile, grant.Count, grant.Limit, dbGrant.TrackId,
                            expirationDate, DateTime.Parse(grant.RevokedAt), provider.Id, grant.Token, grant.Id, status, dbGrant.DurationPerMonth);

                        await _directDebitGrantRepository.UpdateAsync(dbGrant);
                        await _directDebitGrantRepository.SaveChangesAsync();
                    }
                }
            }

            var plans = await _directDebitPlanRepository.ListAsync(new DirectDebitPlanByBankValidityDurationSpec((short)bank.DirectDebitSetting.MaxMandateValidityDurationPerMonth), cancellationToken);
            var response = new PlanViewModel
            {
                Bank = new BankDataViewModel { Id = bank.Id, Name = bank.Name },
                BankPlans = plans?.Select(t => new BankPlanViewModel
                {
                    PlanId = t.Id,
                    DurationPerMonth = t.DurationPerMonth,
                    MaxWithdrawalAmountPerDay = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay
                }).ToList(),
                DirectDebitGrantCount = grants?.Count ?? 0,
                GrantDetails = grants?.Select(t => new GrantDataViewModel
                {
                    CraetionDateTime = t.CreationDate,
                    AccountNumber = t.AccountNumber,
                    AmountLimitPerTransaction = t.AmountLimitPerTransaction,
                    RemainingDays = (t.ExpirationDate - DateTime.Now).Days
                }).ToList(),
            };

            return Result<PlanViewModel>.SuccessResult(response);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<PlanViewModel>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}