using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.Auth;
using Daryaftyar.Application.UseCases.Exceptions;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly IAggregateRepository<DaryaftyarUser> _repository;
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAggregateRepository<DaryaftyarUser> repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand query, CancellationToken cancellationToken)
        {
            var spec = new DaryaftyarUserByLoginSpec(query.Login);
            var user = await _repository.GetBySpecAsync(spec, cancellationToken)
                ?? throw new UserAuthenticationException("Invalid credentials.");

            if (!PasswordManager.VerifyHashedPassword(user.Credentials.Password, query.Password))
                throw new UserAuthenticationException("Invalid credentials.");

            if (!user.IsActive)
                throw new UserAuthenticationException("User is inactive.");

            var token = _authService.GenerateSecurityToken(user.Id, user.Email, $"{user.FirstName} {user.LastName}");

            return new LoginCommandResponse(token);
        }
    }
}