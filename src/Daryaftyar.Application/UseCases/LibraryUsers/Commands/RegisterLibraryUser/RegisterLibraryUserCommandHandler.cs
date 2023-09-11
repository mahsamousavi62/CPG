using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.Auth;
using Daryaftyar.Application.UseCases.Exceptions;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.DaryaftyarUsers.Commands.RegisterDaryaftyarUser
{
    public class RegisterDaryaftyarUserCommandHandler : IRequestHandler<RegisterDaryaftyarUserCommand>
    {
        private readonly IAggregateRepository<DaryaftyarUser> _repository;

        public RegisterDaryaftyarUserCommandHandler(IAggregateRepository<DaryaftyarUser> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RegisterDaryaftyarUserCommand command, CancellationToken cancellationToken)
        {
            var spec = new DaryaftyarUserByEmailSpec(command.Email);
            var existingDaryaftyarUser = await _repository.GetBySpecAsync(spec, cancellationToken);
            
            if (existingDaryaftyarUser is not null)
                throw new DaryaftyarUserAlreadyExistsException(command.Email);

            var hashedPassword = PasswordManager.HashPassword(command.Password); // Should we do it here or is it a domain responsibility to hash password? I guess it's domain's
            var credentials = new UserCredential(command.Login, hashedPassword);
            var name = new Name(command.FirstName, command.LastName);
            var email = new Email(command.Email);

            var daryaftyarUser = DaryaftyarUser.Create(credentials, name, email);

            await _repository.AddAsync(daryaftyarUser, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}