using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Application.UseCases.DirectDebit.ViewModels;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetUserDirectDebitPlanCommandHandler(IDirectDebitFactory DirectDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<Provider> providerRepository,
    IAggregateRepository<DirectDebitPlan> planRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogger<GetUserPhoneNumbersCommandHandler> logger,
    IAuthenticationService authenticationService,
    ICurrentUser user,
    IDirectDebitFactory directDebitFactory
    ) : IRequestHandler<ConfirmGrantCommand, Result<ConfirmGrantResponseViewModel>>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IAggregateRepository<DirectDebitPlan> _DirectDebitPlanRepository = planRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _DirectDebitGrantRepository = grantRepository;
    private readonly ILogger<GetUserPhoneNumbersCommandHandler> _logger = logger;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ICurrentUser _user = user;
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;

    public async Task<Result<ConfirmGrantResponseViewModel>> Handle(ConfirmGrantCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetBySpecAsync(new BankWithProviderByIdSpec(request.model.BankId), cancellationToken);
            if (bank == null)
            {
                throw new BankNotFoundException(request.model.BankId);
            }
            if (bank.IsActive is false)
            {
                throw new DirectDebitBankIsInactiveException();
            }
            if (bank.HasDirectDebitFeature is not true)
            {
                throw new BankDirectDebitFeatureIsDisabledException();
            }
            var provider = bank.DirectDebitSetting.Provider;
            if (provider.IsActive is false)
            {
                throw new BankProviderIsInactiveException();
            }
            if (provider.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false)
            {
                throw new ProviderDoesNotContainDirectDebitMethodException();
            }
            if (bank.DirectDebitSetting.IsActive is false)
            {
                throw new ProviderBankIsInactiveException();
            }

            var plan = await _DirectDebitPlanRepository.GetByIdAsync(request.model.PlanId, cancellationToken);
            if (plan == null)
            {
                throw new PlanNotFoundException(request.model.PlanId);
            }
            if (plan.IsActive is false)
            {
                throw new PlanIsInactiveException();
            }
            if (plan.DurationPerMonth > (short)bank.DirectDebitSetting.MaxMandateValidityDurationPerMonth)
            {
                throw new PlanDurationException();
            }

            var directDebitProvider = _directDebitFactory.GetInstance(provider.ProviderType);

            var tokenResult = await directDebitProvider.GetTokenAsync(new TokenRequest { ProviderData = provider.ProviderData });

            var providerData = JObject.Parse(provider.ProviderData);
            if (providerData["Refresh_Token"].ToString() != tokenResult.RefreshToken)
            {
                providerData["Refresh_Token"] = tokenResult.RefreshToken;
                provider.ProviderData = Newtonsoft.Json.JsonConvert.SerializeObject(providerData);
                await _providerRepository.UpdateAsync(provider);
                await _providerRepository.SaveChangesAsync();
            }

            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
            var name = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.Name);
            var family = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.Surname);
            var nationalCode = await _authenticationService.GetDataFromClaim<string>("NationalCode");

            var storeRequest = new StoreRequest
            {
                AccessToken = tokenResult.AccessToken,
                ProviderData = provider.ProviderData,
                BankCode = bank.DirectDebitSetting.DDBankCode,
                MobileNumber = mobileNumber,
                Limit = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay,
                ExpirationDate = DateTime.Now.AddMonths(plan.DurationPerMonth),
                FullName = $"{name} {family}",
                NationalCode = nationalCode,
            };
            var result = await directDebitProvider.StoreAsync(storeRequest);

            var directDebitGrant = DirectDebitGrant.Create(_user.UserId, bank.Id, null, request.model.PhoneNumber, 1000,
                bank.DirectDebitSetting.MaxWithdrawalAmountPerDay, result.TrackerId, storeRequest.ExpirationDate, null,
                provider.Id, result.Token, null, 0, plan.DurationPerMonth);

            await _DirectDebitGrantRepository.AddAsync(directDebitGrant);
            await _DirectDebitGrantRepository.SaveChangesAsync();

            var response = new ConfirmGrantResponseViewModel { Url = $"{providerData["DD_Grant_Base_URL"]}/{result.Token}" };

            return Result<ConfirmGrantResponseViewModel>.SuccessResult(response);
        }
        catch (DomainException exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<ConfirmGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<ConfirmGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<ConfirmGrantResponseViewModel>.Failure(new Error("1009000", GlobalResource.UnexpectedError));
        }
    }
}