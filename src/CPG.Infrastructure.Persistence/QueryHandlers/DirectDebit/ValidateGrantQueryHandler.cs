using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Application.UseCases.DirectDebit.Queries;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Application.UseCases.DirectDebit.Exceptions;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;
using CPG.Domain.SharedKernel.Helper;

namespace CPG.Infrastructure.Persistence.QueryHandlers.DirectDebit;

public class ValidateGrantQueryHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<DirectDebitGrant> directDebitGrantRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> bankRepository,
    IAuthenticationService authenticationService,
    IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> providerRepository
    ) : IRequestHandler<ValidateGrantQuery, Result<ValidateGrantResponseViewModel>>
{
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = directDebitGrantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.BankAggregate.Bank> _bankRepository = bankRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;
    private const string voided = "باطل";
    private const string activated = "تایید";

    public async Task<Result<ValidateGrantResponseViewModel>> Handle(ValidateGrantQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var directDebitGrant = await _directDebitGrantRepository.GetBySpecAsync(new DirectDebitGrantByTrackIdSpec(request.TrackId), cancellationToken);

            if (directDebitGrant is null)
                throw new NotFoundTrackIdException();
            if (directDebitGrant.Status != DirectDebitGrantStatus.Draft)
                throw new TrackIdInvalidStatusException();

            var provider = directDebitGrant.Provider;
            Domain.AggregateModels.BankAggregate.Bank bank = null;
            switch (provider.ProviderType)
            {
                case ProviderType.Vandar:
                    {
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

                        var req = System.Text.Json.JsonSerializer.Deserialize<VandarValidateGrantViewModel>(request.ValidateGrant.Request);

                        var showRequest = new ShowRequest
                        {
                            MobileNumber = mobileNumber,
                            ProviderData = directDebitGrant.Provider.ProviderData,
                            AccessToken = tokenResult.AccessToken,
                            AuthorizationId = req.AuthorizationId,
                        };
                        var result = await directDebitProvider.ShowAsync(showRequest);

                        bank = await _bankRepository.GetBySpecAsync(new BankByDirectDebitCodeSpec(result.GrantData.BankCode), cancellationToken);

                        if (bank is null)
                        {
                            return Result<ValidateGrantResponseViewModel>.Failure(new Error("2006000", GlobalResource.UnexpectedError));
                        }

                        if (!string.IsNullOrEmpty(req.AuthorizationId))
                        {
                            if (!string.IsNullOrEmpty(result.GrantData.Status))
                            {
                                directDebitGrant.Status = SharedServices.GetVandarDirectDebitGrantStatus(result.GrantData.Status);                                
                            }
                            else
                            {
                                if (result.GrantMessage.Contains(voided))
                                {
                                    directDebitGrant.Status = DirectDebitGrantStatus.Voided;
                                }
                            }
                        }
                        else
                        {
                            directDebitGrant.Status = DirectDebitGrantStatus.CanceledByUser;
                        }
                        if (!string.IsNullOrEmpty(result.GrantData.AccountNumber))
                        {
                            directDebitGrant.AccountNumber = result.GrantData.AccountNumber;
                        }
                        if (!string.IsNullOrEmpty(result.GrantData.Id))
                        {
                            directDebitGrant.AuthorizationId = result.GrantData.Id;
                        }
                        else if (!string.IsNullOrEmpty(req.AuthorizationId))
                        {
                            directDebitGrant.AuthorizationId = req.AuthorizationId;
                        }
                        await _directDebitGrantRepository.UpdateAsync(directDebitGrant);
                        await _directDebitGrantRepository.SaveChangesAsync();

                        if (directDebitGrant.Status == DirectDebitGrantStatus.WaitingForConfirmation)
                        {
                            tokenResult = await directDebitProvider.GetTokenAsync(new TokenRequest { ProviderData = provider.ProviderData });

                            providerData = JObject.Parse(provider.ProviderData);
                            if (providerData["Refresh_Token"].ToString() != tokenResult.RefreshToken)
                            {
                                providerData["Refresh_Token"] = tokenResult.RefreshToken;
                                provider.ProviderData = Newtonsoft.Json.JsonConvert.SerializeObject(providerData);
                                await _providerRepository.UpdateAsync(provider);
                                await _providerRepository.SaveChangesAsync();
                            }

                            var verifyRequest = new VerifyRequest
                            {
                                AccessToken = tokenResult.AccessToken,
                                AuthorizationId = req.AuthorizationId,
                                ProviderData = provider.ProviderData,
                            };
                            var verifyResult = await directDebitProvider.VerifyAsync(verifyRequest);

                            if (verifyResult.GrantStatus == 1)
                            {
                                directDebitGrant.Status = DirectDebitGrantStatus.Activated;
                            }
                            else if (verifyResult.GrantStatus == 0)
                            {
                                if (verifyResult.GrantMessage.Contains(voided))
                                {
                                    directDebitGrant.Status = DirectDebitGrantStatus.Voided;
                                }
                                else if (verifyResult.GrantMessage.Contains(activated))
                                {
                                    directDebitGrant.Status = DirectDebitGrantStatus.Activated;
                                }
                            }
                            await _directDebitGrantRepository.UpdateAsync(directDebitGrant);
                            await _directDebitGrantRepository.SaveChangesAsync();
                        }

                        break;
                    }

            }

            return Result<ValidateGrantResponseViewModel>.SuccessResult(new ValidateGrantResponseViewModel
            {
                BankLogo = bank?.LogoAddress,
                BankName = bank?.Name,
                AccountNumber = directDebitGrant.AccountNumber,
                Status = directDebitGrant.Status,
                MaxWithdrawalAmountPerDay = bank.DirectDebitSetting.MaxWithdrawalAmountPerDay,
                DurationPerMonth = directDebitGrant.DurationPerMonth,
            });
        }
        catch (DomainException exc)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error("1010000", GlobalResource.UnexpectedError));
        }
    }
}
