using CPG.Application.UseCases.IPGTypes.Queries;
using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.IPGType;

public class GetActiveIPGTypesQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetActiveIPGTypesQuery, Result<IReadOnlyCollection<IPGTypeViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<IPGTypeViewModel>>> Handle(GetActiveIPGTypesQuery request, CancellationToken cancellationToken)
    {
        var ipgTypes = await _context.IPGTypeReadModels.Where(t => t.IsActive).ToListAsync(cancellationToken: cancellationToken);

        var viewModels = await Task.WhenAll(ipgTypes.Select(async x => new IPGTypeViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(x.Logo),
            Code = x.Code,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<IPGTypeViewModel>>.SuccessResult(viewModels);
    }
}

