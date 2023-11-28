using Ardalis.GuardClauses;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.Providers.Exceptions;
using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Provider;

public class GetProviderQueryHandler(ReadDbContext context) : IRequestHandler<GetProviderQuery, ProviderViewModel>
{
    private readonly ReadDbContext _context = context;

    public async Task<ProviderViewModel> Handle(GetProviderQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.ProviderId, nameof(request.ProviderId));

        var provider = await _context.ProviderReadModels.FirstOrDefaultAsync(t => t.Id == request.ProviderId);

        if (provider == null)
            throw new ProviderNotFoundException(request.ProviderId);

        var providerModel = new ProviderViewModel
        {
            Id = provider.Id,
            PersianName = provider.PersianName,
            EnglishName = provider.EnglishName,
            Logo = provider.Logo,
            ProviderData = provider.ProviderData,
            ProviderType = provider.ProviderType,
        };

        return providerModel;
    }
}
