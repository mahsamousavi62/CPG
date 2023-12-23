using CPG.Application.Shared;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Idp;
using MediatR;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace CPG.Application.UseCases.Users.Commands
{
    public class CreateUserCommandHandler(IIdpProvider idpClient, IAggregateRepository<User> repository)
            : IRequestHandler<CreateUserCommnad>
    {
        private readonly IAggregateRepository<User> _repository = repository;
        private readonly IIdpProvider _idpClient = idpClient;

        public async Task Handle(CreateUserCommnad request, CancellationToken cancellationToken)
        {
            
            var idpUserProfileResponse = await _idpClient.GetUserProfile(request.IdpId);

            if (idpUserProfileResponse.OperationResult == Enums.OperationResult.Failed)
                throw new IdpUserProfileException(idpUserProfileResponse.Error);

            var idpUserProfile = idpUserProfileResponse.Data;
            var name = new Name(idpUserProfile.Result.PrivatePerson.FirstName, idpUserProfile.Result.PrivatePerson.LastName);
            var phoneNumber = new PhoneNumber(idpUserProfile.Result.Mobile.ToString());
            var nationalCode = new NationalCode(idpUserProfile.Result.UniqueIdentifier);

            var spec = new UserByIDPIdSpec(request.IdpId);
            var existingUser = await _repository.GetBySpecAsync(spec, cancellationToken);
            var userToUpdate = existingUser;

            if (userToUpdate == null)
            {
                var nationalCodeSpec = new UserByNationalCodeSpec(idpUserProfile.Result.UniqueIdentifier);
                userToUpdate = await _repository.GetBySpecAsync(nationalCodeSpec);

                if (userToUpdate == null)
                {
                    userToUpdate = User.Create(request.IdpId, nationalCode, name, phoneNumber, Enums.UserRoleType.CustomerUser);
                    await _repository.AddAsync(userToUpdate, cancellationToken);
                }
            }
            else
            {
                User.Update(userToUpdate, name, phoneNumber);
                await _repository.UpdateAsync(userToUpdate, cancellationToken);
            }
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
