using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Queries;

public record GetAllUserQuery : IRequest<Result<UserViewModel>>;

