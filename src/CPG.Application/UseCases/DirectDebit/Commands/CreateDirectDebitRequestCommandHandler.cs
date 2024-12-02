using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using System.Security.Claims;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class CreateDirectDebitRequestCommandHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<Bank> bankRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Provider> providerRepository,
    IAuthenticationService authenticationService
    ) : IRequestHandler<CreateDirectDebitRequestCommand, Result<bool>>
{
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<bool>> Handle(CreateDirectDebitRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bank = await _bankRepository.GetBySpecAsync(new BankByIdSpec(request.model.BankId));
            var provider = await _providerRepository.GetByIdAsync(bank.DirectDebitSetting.ProviderId);
            var factory = _directDebitFactory.GetInstance(provider.ProviderType);
            var mobileNumber = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.MobilePhone);
            var name = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.Name);
            var family = await _authenticationService.GetDataFromClaim<string>(ClaimTypes.Surname);
            var nationalCode = await _authenticationService.GetDataFromClaim<string>("NationalCode");

            var storeRequest = new StoreRequest
            {
                ProviderData = provider.ProviderData,
                BankCode = bank.DirectDebitSetting.DDBankCode,
                FullName = $"{name} {family}",
                MobileNumber = mobileNumber,
                Limit = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay,
                NationalCode = nationalCode,
                //ExpirationDate = DateTime.Now.AddMonths()
            };
            var tokenData = await factory.StoreAsync(storeRequest);
            switch (provider.ProviderType)
            {
                case Enums.ProviderType.Vandar:
                    {
                        //ToDo: save RefreshToken
                        //provider.ProviderData = System.Text.Json.JsonSerializer.Serialize(new { Refresh_Token = tokenData.RefreshToken });
                        break;
                    }
                case Enums.ProviderType.AsanPardakht:
                    break;
                case Enums.ProviderType.Sep:
                    break;
                case Enums.ProviderType.Pec:
                    break;
                default:
                    break;
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception exc)
        {
            return Result<bool>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}