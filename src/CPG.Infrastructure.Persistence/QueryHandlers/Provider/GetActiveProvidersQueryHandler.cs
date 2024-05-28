using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
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

public class GetActiveProvidersQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetActiveProvidersQuery, Result<IReadOnlyCollection<ProviderViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<ProviderViewModel>>> Handle(GetActiveProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _context.ProviderReadModels.Where(t => t.IsActive).ToListAsync(cancellationToken: cancellationToken);

        var viewModels = await Task.WhenAll(providers.Select(async x => new ProviderViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await General.GetLogo( _minioProvider,x.Logo),
            ProviderData = x.ProviderData,
            ProviderType = x.ProviderType,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<ProviderViewModel>>.SuccessResult(viewModels);
    }
}

