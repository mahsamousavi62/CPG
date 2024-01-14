using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Providers.Queries;

public record GetActiveProvidersQuery : IRequest<Result<IReadOnlyCollection<ProviderViewModel>>>;