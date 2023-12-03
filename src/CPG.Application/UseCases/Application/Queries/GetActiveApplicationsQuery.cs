using CPG.Application.UseCases.Application.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Application.Queries;

public class GetActiveApplicationsQuery : IRequest<IReadOnlyCollection<ApplicationViewModel>>
{
}
