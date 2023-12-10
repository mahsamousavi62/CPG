using CPG.Application.UseCases.IPGTypes.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.IPGTypes.Queries;

public class GetAllIPGTypesQuery : IRequest<IReadOnlyCollection<IPGTypeViewModel>>
{
}