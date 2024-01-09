using Ardalis.GuardClauses;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Application;

public class GetApplicationQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetApplicationQuery, Result<ApplicationViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<ApplicationViewModel>> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.AppId, nameof(request.AppId));

        var app = await _context.ApplicationReadModels.FirstOrDefaultAsync(t => t.Id == request.AppId);

        if (app == null)
            throw new ApplicationNotFoundException(request.AppId);

        var appModel = new ApplicationViewModel
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
        };

        return Result<ApplicationViewModel>.SuccessResult(appModel);
    }
}
