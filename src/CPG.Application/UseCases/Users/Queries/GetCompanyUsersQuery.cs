using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Users.Queries;

public record GetCompanyUsersQuery : IRequest<Result<IReadOnlyCollection<UserCompanyViewModel>>>;