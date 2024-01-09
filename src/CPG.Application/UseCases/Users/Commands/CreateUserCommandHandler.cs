using CPG.Application.Shared;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Idp;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace CPG.Application.UseCases.Users.Commands;

public class CreateUserCommandHandler(IIdpProvider idpClient, IAggregateRepository<User> repository, IAuthenticationService authenticationService)
        : IRequestHandler<CreateUserCommnad, Result<long>>
{
    private readonly IAggregateRepository<User> _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IIdpProvider _idpClient = idpClient;

    public async Task<Result<long>> Handle(CreateUserCommnad request, CancellationToken cancellationToken)
    {
        try
        {
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);
            var idpUserProfileResponse = await _idpClient.GetUserProfile(sub);

            if (idpUserProfileResponse.OperationResult == Enums.OperationResult.Failed)
                throw new IdpUserProfileException(idpUserProfileResponse.Error);

            var idpUserProfile = idpUserProfileResponse.Data;
            var name = new Name(idpUserProfile.Result.PrivatePerson.FirstName, idpUserProfile.Result.PrivatePerson.LastName);
            var phoneNumber = new PhoneNumber(idpUserProfile.Result.Mobile.ToString());
            var nationalCode = new NationalCode(idpUserProfile.Result.UniqueIdentifier);

            var spec = new UserByIDPIdSpec(sub);
            var userToUpdate = await _repository.GetBySpecAsync(spec, cancellationToken);

            if (userToUpdate == null)
            {
                var nationalCodeSpec = new UserByNationalCodeSpec(idpUserProfile.Result.UniqueIdentifier);
                userToUpdate = await _repository.GetBySpecAsync(nationalCodeSpec);

                    if (userToUpdate == null)
                    {
                        userToUpdate = User.Create(sub, nationalCode, name, phoneNumber, Enums.UserRoleType.CustomerUser);
                        await _repository.AddAsync(userToUpdate, cancellationToken);
                    }
                    else
                    {
                        userToUpdate = User.Update(userToUpdate, name, phoneNumber);
                        await _repository.UpdateAsync(userToUpdate, cancellationToken);
                    }
                }
                else
                {
                    userToUpdate = User.Update(userToUpdate, name, phoneNumber);
                    await _repository.UpdateAsync(userToUpdate, cancellationToken);
                }
                await _repository.SaveChangesAsync(cancellationToken);
                return Result<long>.SuccessResult(userToUpdate.Id);
            }
            catch (DomainException exc)
            {
                return Result<long>.Failure(new Error((exc as dynamic).Code, exc.Message));
            }
            catch (AppException exc)
            {
                return Result<long>.Failure(new Error((exc as dynamic).Code, exc.Message));
            }
            catch (Exception exc)
            {
                return Result<long>.Failure(new Error(exc.Source, exc.Message));
            }
        }
    }
}
