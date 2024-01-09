using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Application;

public class GetActiveApplicationsQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllApplicationsQuery, Result<IReadOnlyCollection<ApplicationViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<ApplicationViewModel>>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
    {
        var apps = await _context.ApplicationReadModels
            .Include(x => x.ApplicationIdentifiers)
            .Include(x => x.ApplicationCallbackUrls)
            .ToListAsync(cancellationToken: cancellationToken);

        var data = await Task.WhenAll(apps.Select(async app => new ApplicationViewModel
        {
            Id = app.Id,
            PersianName = app.PersianName,
            EnglishName = app.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(app.Logo),
            CreationDate = app.CreationDate,
            ModificationDate = app.ModificationDate,
            IsActive = app.IsActive,
            ResponseApiUrl = app.ResponseApiUrl,
            IdpClientIds = app.ApplicationIdentifiers?.ToDictionary(key => key.Id, value => value.IdpClientId),
            CallbackUrls = app.ApplicationCallbackUrls?.ToDictionary(key => key.Id, value => value.CallbackUrl)
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<ApplicationViewModel>>.SuccessResult(data);
    }
}