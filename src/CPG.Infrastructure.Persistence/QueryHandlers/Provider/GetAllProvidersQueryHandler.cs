using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Provider;

public class GetAllProvidersQueryHandler(ReadDbContext context) : IRequestHandler<GetAllProvidersQuery, IReadOnlyCollection<ProviderViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<ProviderViewModel>> Handle(GetAllProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _context.ProviderReadModels.Select(x => new ProviderViewModel
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

