using CPG.Application.Auth;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler(IAggregateRepository<User> repository, IAuthService authService) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    private readonly IAggregateRepository<User> _repository = repository;
    private readonly IAuthService _authService = authService;

    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand query, CancellationToken cancellationToken)
    {
        //var spec = new UserByLoginSpec(query.Login);
        //var user = await _repository.GetBySpecAsync(spec, cancellationToken)
        //    ?? throw new UserAuthenticationException("Invalid credentials.");

        //if (!PasswordManager.VerifyHashedPassword(user.Credentials.Password, query.Password))
        //    throw new UserAuthenticationException("Invalid credentials.");

        //if (!user.IsActive)
        //    throw new UserAuthenticationException("User is inactive.");

        //var token = _authService.GenerateSecurityToken(user.Id, user.Email, $"{user.FirstName} {user.LastName}");

        //return new LoginCommandResponse(token);
        return Result<LoginCommandResponse>.SuccessResult(new LoginCommandResponse(null));
    }
}