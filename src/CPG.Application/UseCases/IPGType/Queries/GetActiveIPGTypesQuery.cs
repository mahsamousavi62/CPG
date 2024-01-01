using CPG.Application.UseCases.IPGTypes.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.IPGTypes.Queries;

public class GetActiveIPGTypesQuery : IRequest<IReadOnlyCollection<IPGTypeViewModel>>
{
}
