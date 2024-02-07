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
using CPG.Application.UseCases.Ipg.ViewModels;

namespace CPG.Infrastructure.Persistence.QueryHandlers.DirectDebit;

public class ValidateGrantQueryHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<DirectDebitGrant> directDebitGrantRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAuthenticationService authenticationService,
    IAggregateRepository<CPG.Domain.AggregateModels.ProviderAggregate.Provider> providerRepository
    ) : IRequestHandler<ValidateGrantQuery, Result<ValidateGrantResponseViewModel>>
{
    private readonly IDirectDebitFactory _directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = directDebitGrantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ProviderAggregate.Provider> _providerRepository = providerRepository;

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

                        if (!string.IsNullOrEmpty(req.AuthorizationId))
                        {
                            if (!string.IsNullOrEmpty(result.GrantData.Status))
                            {
                                switch (result.GrantData.Status)
                                {
                                    case "PENDING_VERIFY":
                                        {
                                            directDebitGrant.Status = DirectDebitGrantStatus.WaitingForConfirmation;
                                            break;
                                        }
                                    case "ACTIVE":
                                        {
                                            directDebitGrant.Status = DirectDebitGrantStatus.Activated;
                                            break;
                                        }
                                    case "REVOKED":
                                        {
                                            directDebitGrant.Status = DirectDebitGrantStatus.Removed;
                                            break;
                                        }
                                    case "REVOKED_AUTO":
                                        {
                                            directDebitGrant.Status = DirectDebitGrantStatus.Removed;
                                            break;
                                        }
                                    case "EXPIRED":
                                        {
                                            directDebitGrant.Status = DirectDebitGrantStatus.Expired;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                if (result.GrantMessage.Contains("مجوز باطل شده است"))
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

                        break;
                    }

            }
            return Result<ValidateGrantResponseViewModel>.SuccessResult(new ValidateGrantResponseViewModel
            {

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
