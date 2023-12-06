using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Provider;

public class GetAllProvidersQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllProvidersQuery, IReadOnlyCollection<ProviderViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyCollection<ProviderViewModel>> Handle(GetAllProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _context.ProviderReadModels.ToListAsync(cancellationToken: cancellationToken);
        
        return await Task.WhenAll(providers.Select(async x => new ProviderViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(x.Logo),
            ProviderData = x.ProviderData,
            ProviderType = x.ProviderType,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        })).ConfigureAwait(false);
    }
}