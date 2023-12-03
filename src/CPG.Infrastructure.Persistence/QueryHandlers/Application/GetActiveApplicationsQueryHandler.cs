using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Application;

public class GetActiveApplicationsQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllApplicationsQuery, IReadOnlyCollection<ApplicationViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyCollection<ApplicationViewModel>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
    {
        var apps = await _context.ApplicationReadModels
            .Include(m => m.ApplicationIdentifiers)
            .ToListAsync(cancellationToken: cancellationToken);

        return await Task.WhenAll(apps.Select(async company => new ApplicationViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(company.Logo),
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive,
            ResponseApiUrl = company.ResponseApiUrl,
            IdpClientIds = company.ApplicationIdentifiers.ToDictionary(key => key.Id, value => value.IdpClientId)
        })).ConfigureAwait(false);
    }
}