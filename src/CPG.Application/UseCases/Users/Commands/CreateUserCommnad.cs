using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateUserCommnad : IRequest<Result<long>>;
