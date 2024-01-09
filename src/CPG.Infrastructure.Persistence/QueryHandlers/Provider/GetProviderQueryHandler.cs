using Ardalis.GuardClauses;
using CPG.Application.UseCases.Providers.Exceptions;
using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Provider;

public class GetProviderQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetProviderQuery, Result<ProviderViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<ProviderViewModel>> Handle(GetProviderQuery request, CancellationToken cancellationToken)
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
            Logo = await _minioProvider.PresignedGetObject(provider.Logo),
            ProviderData = provider.ProviderData,
            ProviderType = provider.ProviderType,
            IpgVerificationTimeLimit = provider.IpgVerificationTimeLimit,
            IpgBaseUrl = provider.IpgBaseUrl,
        };

        return Result<ProviderViewModel>.SuccessResult(providerModel);
    }
}