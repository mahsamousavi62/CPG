using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.IPGTypes.Queries;

public record GetAllIPGTypesQuery : IRequest<Result<IReadOnlyCollection<IPGTypeViewModel>>>;