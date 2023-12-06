using CPG.Application.UseCases.IPGType.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.IPGType.Queries;

public class GetActiveIPGTypesQuery : IRequest<IReadOnlyCollection<IPGTypeViewModel>>
{
}
