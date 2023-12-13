using CPG.Application.Shared;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ClientFactory;
using MediatR;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;


namespace CPG.Application.UseCases.Users.Commands
{
    public class CreateUserCommandHandler(IIdpClient idpClient, IMediator mediator, IAggregateRepository<User> repository, IHttpClientFactoryService httpClientFactoryService) : IRequestHandler<CreateUserCommnad>
    {
        private readonly IAggregateRepository<User> _repository = repository;
        private readonly IIdpClient _idpClient = idpClient;
        private readonly IMediator _mediator = mediator;
        private readonly IHttpClientFactoryService _httpClientFactoryService = httpClientFactoryService;

        public async Task Handle(CreateUserCommnad request, CancellationToken cancellationToken)
        {
            var idpUserProfileResponse = await _idpClient.GetUserProfile(request.IdpId);

            if (idpUserProfileResponse.OperationResult==Enums.OperationResult.Failed)
                throw new System.Exception(idpUserProfileResponse.Error);
            
            var idpUserProfile = idpUserProfileResponse.Data;
            //var spec = new UserByIDPIdSpec(idpUserProfile.Result.Id);
            //var existingUser = await _repository.GetBySpecAsync(spec, cancellationToken);
            // var existingUser = await _repository.GetByIdAsync<long>(1, cancellationToken);//TODO:

            var name = new Name(idpUserProfile.Result.PrivatePerson.FirstName, idpUserProfile.Result.PrivatePerson.LastName);
            var phoneNumber = new PhoneNumber(idpUserProfile.Result.Mobile.ToString());
            var nationalCode = new NationalCode(idpUserProfile.Result.UniqueIdentifier);

            //if (existingUser is null)
            //{
            //    var user = User.Create(idpUserProfile.Result.Id, nationalCode, name, phoneNumber,(short)Enums.UserRoleType.Customer);

            //   await _repository.AddAsync(user, cancellationToken);
            //}
            //else
            //{
            //    var user = User.Update(name, idpUserProfile.Result.Mobile.ToString());

            //    await _repository.UpdateAsync(user, cancellationToken);
            //}

            try
            {
                //    await _repository.SaveChangesAsync(cancellationToken); TODO:
            }
            catch (System.Exception eX)
            {

                var message = eX.Message;
            }

        }
    }
}
