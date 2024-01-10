using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateRedirectUrlCommnad(string id) : IRequest<Result<string>>;
