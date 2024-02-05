using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class SetUserDirectDebitPlanCommandHandler(IDirectDebitFactory DirectDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<DirectDebitPlan> planRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogger<GetUserPhoneNumbersCommandHandler> logger,
    IAuthenticationService authenticationService,
    ICurrentUser user
    ) : IRequestHandler<SetUserDirectDebitPlanCommand, Result<bool>>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<DirectDebitPlan> _DirectDebitPlanRepository = planRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _DirectDebitGrantRepository = grantRepository;
    private readonly ILogger<GetUserPhoneNumbersCommandHandler> _logger = logger;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ICurrentUser _user = user;

    public async Task<Result<bool>> Handle(SetUserDirectDebitPlanCommand request, CancellationToken cancellationToken)
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
            if (bank.DirectDebitSetting.Provider.IsActive is false)
            {
                throw new BankProviderIsInactiveException();
            }
            if (bank.DirectDebitSetting.Provider.PaymentMethods?.Any(t => t.MethodType == Enums.PaymentMethodType.DirectDebit) is false)
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
            if(plan.DurationPerMonth > (short)bank.DirectDebitSetting.MaxMandateValidityDurationPerMonth)
            {
                throw new PlanDurationException();
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (DomainException exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<bool>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<bool>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc);
            return Result<bool>.Failure(new Error("1009000", GlobalResource.UnexpectedError));
        }
    }
}