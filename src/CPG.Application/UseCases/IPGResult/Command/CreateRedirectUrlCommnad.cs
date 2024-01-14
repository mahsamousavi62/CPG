using CPG.Application.UseCases.IPGResult;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Commands;

public record CreateRedirectUrlCommnad(RedirectViewModel model, string id) : IRequest<Result<string>>;
