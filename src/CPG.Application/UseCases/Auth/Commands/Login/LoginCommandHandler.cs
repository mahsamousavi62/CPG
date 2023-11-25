using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Auth;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler(IAggregateRepository<CPGUser> repository, IAuthService authService) : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    private readonly IAggregateRepository<CPGUser> _repository = repository;
    private readonly IAuthService _authService = authService;

    public async Task<LoginCommandResponse> Handle(LoginCommand query, CancellationToken cancellationToken)
    {
        var spec = new CPGUserByLoginSpec(query.Login);
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