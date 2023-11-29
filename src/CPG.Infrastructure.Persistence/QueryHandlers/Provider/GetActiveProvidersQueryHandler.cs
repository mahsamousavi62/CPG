using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
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

public class GetActiveProvidersQueryHandler(ReadDbContext context) : IRequestHandler<GetActiveProvidersQuery, IReadOnlyCollection<ProviderViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<ProviderViewModel>> Handle(GetActiveProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _context.ProviderReadModels.Where(t => t.IsActive).Select(x => new ProviderViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = x.Logo,
            ProviderData = x.ProviderData,
            ProviderType = x.ProviderType,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        }).ToListAsync(cancellationToken: cancellationToken);

        return providers;
    }
}

