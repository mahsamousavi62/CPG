using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using System.Security.Claims;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetUserPhoneNumbersCommandHandler(IDirectDebitFactory DirectDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<DirectDebitPlan> planRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    ILogService logService,
    IAuthenticationService authenticationService,
    ICurrentUser user
    ) : IRequestHandler<GetUserPhoneNumbersCommand, Result<IReadOnlyCollection<UserPhoneNumberViewModel>>>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<DirectDebitPlan> _DirectDebitPlanRepository = planRepository;
    private readonly IAggregateRepository<DirectDebitGrant> _DirectDebitGrantRepository = grantRepository;
    private readonly ILogService _logService = logService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly ICurrentUser _user = user;

    public async Task<Result<IReadOnlyCollection<UserPhoneNumberViewModel>>> Handle(GetUserPhoneNumbersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetBySpecAsync(new BankByIdSpec(request.model.BankId), cancellationToken);
            if (bank == null)
            {
                throw new BankNotFoundException(request.model.BankId);
            }
            var plan = await _DirectDebitPlanRepository.GetByIdAsync(request.model.PlanId, cancellationToken);
            if (plan == null)
            {
                throw new PlanNotFoundException(request.model.PlanId);
            }
            var response = new List<UserPhoneNumberViewModel>();
            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
            if (mobileNumber is not null)
            {
                response.Add(new UserPhoneNumberViewModel() { PhoneNumber = mobileNumber, IsDefault = true });
            }
            var grants = await _DirectDebitGrantRepository.ListAsync(new UserDirectDebitGrantSpec(_user.UserId), cancellationToken);
            if (grants?.Any() is true)
            {
                var phoneNumbers = grants.Where(t => t.PhoneNumber != mobileNumber).Select(t => t.PhoneNumber).Distinct();
                response.AddRange(phoneNumbers.Select(t => new UserPhoneNumberViewModel { PhoneNumber = t }));
                var defaultNumber = grants.FirstOrDefault(t => t.BankId == request.model.BankId);
                if (defaultNumber != null)
                {
                    response.ForEach(t => t.IsDefault = false);
                    response.FirstOrDefault(t => t.PhoneNumber == defaultNumber.PhoneNumber).IsDefault = true;
                }
            }

            return Result<IReadOnlyCollection<UserPhoneNumberViewModel>>.SuccessResult(response);
        }
        catch (Exception exc)
        {
            var callLog = CallLogModel.CreateError(
                serviceName: nameof(GetUserPhoneNumbersCommandHandler),
                providerName: "DirectDebit",
                requestUri: nameof(GetUserPhoneNumbersCommand),
                requestBody: Newtonsoft.Json.JsonConvert.SerializeObject(request),
                responseBody: exc.Message,
                exception: exc,
                serviceType: Enums.ServiceType.DirectDebit,
                providerType: Enums.ProviderTypeInLog.Vandar,
                auditType: Enums.AuditType.Application,
                userId: 1
            );
            _logService.LogError(callLog);
            return Result<IReadOnlyCollection<UserPhoneNumberViewModel>>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}