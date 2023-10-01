using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Auth;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CPGUsers.Commands.RegisterCPGUser
{
    public class RegisterCPGUserCommandHandler : IRequestHandler<RegisterCPGUserCommand>
    {
        private readonly IAggregateRepository<CPGUser> _repository;

        public RegisterCPGUserCommandHandler(IAggregateRepository<CPGUser> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RegisterCPGUserCommand command, CancellationToken cancellationToken)
        {
            var spec = new CPGUserByEmailSpec(command.Email);
            var existingCPGUser = await _repository.GetBySpecAsync(spec, cancellationToken);

            if (existingCPGUser is not null)
                throw new CPGUserAlreadyExistsException(command.Email);

            var hashedPassword = PasswordManager.HashPassword(command.Password); // Should we do it here or is it a domain responsibility to hash password? I guess it's domain's
            var credentials = new UserCredential(command.Login, hashedPassword);
            var name = new Name(command.FirstName, command.LastName);
            var email = new Email(command.Email);

            var cpgUser = CPGUser.Create(credentials, name, email);

            await _repository.AddAsync(cpgUser, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}