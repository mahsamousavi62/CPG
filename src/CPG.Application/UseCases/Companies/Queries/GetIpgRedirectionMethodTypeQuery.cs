using System.Collections.Generic;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Companies.Queries;

public record GetIpgRedirectionMethodTypeQuery : IRequest<Result<Dictionary<int, string>>>;