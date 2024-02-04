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

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetDirectDebitPlansCommandHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<DirectDebitPlan> planRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogger<GetDirectDebitPlansCommandHandler> logger,
    ICurrentUser user
    ) : IRequestHandler<GetDirectDebitPlansCommand, Result<PlanViewModel>>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<DirectDebitPlan> _directDebitPlanRepository = planRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = grantRepository;
    private readonly ILogger<GetDirectDebitPlansCommandHandler> _logger = logger;
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
            var plans = await _directDebitPlanRepository.ListAsync(new DirectDebitPlanByBankValidityDurationSpec((short)bank.DirectDebitSetting.MaxMandateValidityDurationPerMonth),cancellationToken);            
            var grants = await _directDebitGrantRepository.ListAsync(new DirectDebitGrantByUserAndBankSpec(bank.Id, _user.UserId, bank.DirectDebitSetting.ProviderId), cancellationToken);
            var response = new PlanViewModel
            {
                Bank = new BankDataViewModel { Id = bank.Id, Name = bank.Name },
                BankPlans = plans?.Select(t => new BankPlanViewModel
                {
                    DurationPerMonth = t.DurationPerMonth,
                    MaxWithdrawalAmountPerDay = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay
                }).ToList(),
                DirectDebitGrantCount = grants?.Count ?? 0,
                GrantDetails = grants?.Select(t => new GrantDataViewModel
                {
                    CraetionDateTime = t.CreationDate,
                    AccountNumber = t.AccountNumber,
                    AmountLimitPerTransaction = t.AmountLimitPerTransaction,
                    RemainingDays = (DateTime.Now - t.CreationDate).Days
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