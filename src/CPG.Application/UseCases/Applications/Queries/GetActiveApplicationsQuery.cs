using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Application.Queries;

public class GetActiveApplicationsQuery : IRequest<Result<IReadOnlyCollection<ApplicationViewModel>>>
{
}
