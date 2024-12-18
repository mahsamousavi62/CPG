using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateUserCommand : IRequest<Result<long>>;
